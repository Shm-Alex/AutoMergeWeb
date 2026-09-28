using AutoMergeWeb.Services;

var builder = WebApplication.CreateBuilder(args);

// 1. РЕГИСТРАЦИЯ СЕРВИСА (Именно это исправляет твою ошибку)
builder.Services.AddSingleton<MergeService>();

// 2. Добавляем поддержку MVC (Контроллеры и Представления)
builder.Services.AddControllersWithViews();

var app = builder.Build();

// 3. Настройка конвейера обработки HTTP-запросов
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles(); // Нужно для работы CSS и JS файлов

app.UseRouting();

app.UseAuthorization();

// 4. Настройка маршрутизации: делаем Merge контроллер главным по умолчанию
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Merge}/{action=Index}/{id?}");

app.Run();