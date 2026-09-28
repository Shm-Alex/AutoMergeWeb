using System;
using System.Collections.Generic;
using System.Linq;

namespace AutoMergeWeb.Services
{
    public class MergeService
    {
        public string Merge(string original, string version1, string version2)
        {
            var origLines = original.Split('\n');
            var v1Lines = version1.Split('\n');
            var v2Lines = version2.Split('\n');

            var merged = MergeLines(origLines, v1Lines, v2Lines);
            return string.Join("\n", merged);
        }

        private static bool Eq(string a, string b) => a.Trim() == b.Trim();

        private static List<(int i, int j)> LCS(string[] a, string[] b)
        {
            int n = a.Length, m = b.Length;
            int[,] dp = new int[n + 1, m + 1];

            for (int i = 1; i <= n; i++)
                for (int j = 1; j <= m; j++)
                    if (Eq(a[i - 1], b[j - 1]))
                        dp[i, j] = dp[i - 1, j - 1] + 1;
                    else
                        dp[i, j] = Math.Max(dp[i - 1, j], dp[i, j - 1]);

            var lcs = new List<(int, int)>();
            int ii = n, jj = m;
            while (ii > 0 && jj > 0)
            {
                if (Eq(a[ii - 1], b[jj - 1]) && dp[ii, jj] == dp[ii - 1, jj - 1] + 1)
                {
                    lcs.Add((ii - 1, jj - 1));
                    ii--; jj--;
                }
                else if (dp[ii - 1, jj] >= dp[ii, jj - 1]) ii--;
                else jj--;
            }
            lcs.Reverse();
            return lcs;
        }

        private static string[] Subarray(string[] arr, int start, int end)
        {
            if (start > end) return Array.Empty<string>();
            var res = new string[end - start + 1];
            Array.Copy(arr, start, res, 0, res.Length);
            return res;
        }

        private static bool RegionsEqual(string[] a, string[] b)
        {
            if (a.Length != b.Length) return false;
            for (int i = 0; i < a.Length; i++)
                if (!Eq(a[i], b[i])) return false;
            return true;
        }

        private static void ProcessRegion(string[] orig, string[] v1, string[] v2, List<string> result,
            int origStart, int origEnd, int j1Start, int j1End, int j2Start, int j2End)
        {
            if (origStart > origEnd && j1Start > j1End && j2Start > j2End) return;

            string[] origRegion = Subarray(orig, origStart, origEnd);
            string[] region1 = Subarray(v1, j1Start, j1End);
            string[] region2 = Subarray(v2, j2Start, j2End);

            bool change1 = !RegionsEqual(region1, origRegion);
            bool change2 = !RegionsEqual(region2, origRegion);

            if (!change1 && !change2) result.AddRange(origRegion);
            else if (change1 && !change2) result.AddRange(region1);
            else if (!change1 && change2) result.AddRange(region2);
            else
            {
                if (RegionsEqual(region1, region2)) result.AddRange(region1);
                else
                {
                    result.Add("<<<<<<< Конфликт (версия 1)");
                    result.AddRange(region1);
                    result.Add("=======");
                    result.AddRange(region2);
                    result.Add(">>>>>>> Конфликт (версия 2)");
                }
            }
        }

        private static List<string> MergeLines(string[] orig, string[] v1, string[] v2)
        {
            var lcs1 = LCS(orig, v1);
            var lcs2 = LCS(orig, v2);

            int[] map1 = Enumerable.Repeat(-1, orig.Length).ToArray();
            foreach (var (i, j) in lcs1) map1[i] = j;

            int[] map2 = Enumerable.Repeat(-1, orig.Length).ToArray();
            foreach (var (i, j) in lcs2) map2[i] = j;

            var stable = new List<int>();
            for (int i = 0; i < orig.Length; i++)
                if (map1[i] != -1 && map2[i] != -1) stable.Add(i);

            var result = new List<string>();
            int prevStable = -1, prevJ1 = -1, prevJ2 = -1;

            foreach (int s in stable)
            {
                ProcessRegion(orig, v1, v2, result, prevStable + 1, s - 1, prevJ1 + 1, map1[s] - 1, prevJ2 + 1, map2[s] - 1);
                result.Add(v1[map1[s]]);
                prevStable = s; prevJ1 = map1[s]; prevJ2 = map2[s];
            }

            ProcessRegion(orig, v1, v2, result, prevStable + 1, orig.Length - 1, prevJ1 + 1, v1.Length - 1, prevJ2 + 1, v2.Length - 1);
            return result;
        }
    }
}