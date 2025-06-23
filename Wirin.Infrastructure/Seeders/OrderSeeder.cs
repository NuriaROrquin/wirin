using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using PDFiumSharp;
using System.IO;
using Wirin.Infrastructure.Context;
using Wirin.Infrastructure.Entities;
using Wirin.Infrastructure.Loaders;

namespace Wirin.Infrastructure.Seeders;

public static class OrderSeeder
{
    public static async Task SeedOrdersAsync(IServiceProvider serviceProvider)
    {
        var dbContext = serviceProvider.GetRequiredService<WirinDbContext>();
        var userManager = serviceProvider.GetRequiredService<UserManager<UserEntity>>();

        await SeedOrderDeliveriesAsync(dbContext, userManager);
        await SeedOrdersAsync(dbContext, userManager);
        await SeedOrderSequencesAsync(dbContext);
    }

    private static async Task SeedOrderDeliveriesAsync(WirinDbContext dbContext, UserManager<UserEntity> userManager)
    {
        if (!dbContext.OrderDeliveries.Any())
        {
            // Obtener todos los usuarios de la base de datos por roles
            var alumnos = new List<UserEntity>();
            var bibliotecarios = new List<UserEntity>();

            // Obtener alumnos
            alumnos.Add(await userManager.FindByEmailAsync("juanperez@biblioteca.com"));
            alumnos.Add(await userManager.FindByEmailAsync("carlosrodriguez@biblioteca.com"));
            alumnos.Add(await userManager.FindByEmailAsync("anafernandez@biblioteca.com"));
            alumnos.Add(await userManager.FindByEmailAsync("sofialopez@biblioteca.com"));

            // Obtener bibliotecarios/admin
            bibliotecarios.Add(await userManager.FindByEmailAsync("mariagonzalez@biblioteca.com"));

            // Verificar que existan los usuarios necesarios
            if (alumnos.Any(a => a == null) || bibliotecarios.Any(b => b == null))
            {
                throw new InvalidOperationException("Los usuarios necesarios no existen en la base de datos. Asegúrese de ejecutar el IdentitySeeder primero.");
            }

            // Crear 50 OrderDelivery
            var orderDeliveries = new List<OrderDeliveryEntity>();
            string[] estados = { "Pendiente", "Completada", "Entregado" };
            string[] materias = { "Matemáticas", "Historia", "Ciencias", "Literatura", "Física", "Química", "Biología", "Geografía", "Economía", "Filosofía" };
            Random random = new Random();

            for (int i = 1; i <= 20; i++)
            {
                // Seleccionar un alumno y un bibliotecario aleatorio
                var alumno = alumnos[random.Next(alumnos.Count)];
                var bibliotecario = bibliotecarios[0]; // Solo hay uno en este caso

                // Crear una fecha de entrega aleatoria entre 1 y 60 días en el futuro
                var diasEntrega = random.Next(1, 61);
                
                // Crear un título aleatorio basado en materias
                var materia = materias[random.Next(materias.Length)];
                var titulo = $"Proyecto de {materia} {i}";
                
                // Seleccionar un estado aleatorio
                var estado = estados[random.Next(estados.Length)];

                orderDeliveries.Add(new OrderDeliveryEntity
                {
                    Title = titulo,
                    Status = estado,
                    StudentUserId = alumno.Id,
                    UserId = bibliotecario.Id,
                    DeliveryDate = DateTime.UtcNow.AddDays(diasEntrega)
                });
            }

            await dbContext.OrderDeliveries.AddRangeAsync(orderDeliveries);
            await dbContext.SaveChangesAsync();
        }
    }

    private static async Task SeedOrdersAsync(WirinDbContext dbContext, UserManager<UserEntity> userManager)
    {
        if (!dbContext.Orders.Any())
        {
            // Obtener usuarios de la base de datos
            var bibliotecarios = new List<UserEntity> { await userManager.FindByEmailAsync("mariagonzalez@biblioteca.com") };
            
            var voluntarios = new List<UserEntity>
            {
                await userManager.FindByEmailAsync("lionelpares@biblioteca.com"),
                await userManager.FindByEmailAsync("martinalopez@biblioteca.com"),
                await userManager.FindByEmailAsync("javiertorres@biblioteca.com")
            };
            
            var alumnos = new List<UserEntity>
            {
                await userManager.FindByEmailAsync("juanperez@biblioteca.com"),
                await userManager.FindByEmailAsync("carlosrodriguez@biblioteca.com"),
                await userManager.FindByEmailAsync("anafernandez@biblioteca.com"),
                await userManager.FindByEmailAsync("sofialopez@biblioteca.com")
            };
            
            var revisores = new List<UserEntity>
            {
                await userManager.FindByEmailAsync("joseruiz@biblioteca.com"),
                await userManager.FindByEmailAsync("lauracastillo@biblioteca.com"),
                await userManager.FindByEmailAsync("rodrigomendez@biblioteca.com")
            };

            // Verificar que existan los usuarios necesarios
            if (bibliotecarios.Any(b => b == null) || voluntarios.Any(v => v == null) || 
                alumnos.Any(a => a == null) || revisores.Any(r => r == null))
            {
                throw new InvalidOperationException("Los usuarios necesarios no existen en la base de datos. Asegúrese de ejecutar el IdentitySeeder primero.");
            }

            // Crear órdenes para cada OrderDelivery (al menos 10 por cada una)
            var orderDeliveries = dbContext.OrderDeliveries.ToList();
            var orders = new List<OrderEntity>();
            Random random = new Random();

            // Datos para generar órdenes aleatorias
            string[] estados = { "Pendiente", "En Proceso", "En Revisión", "Aprobada", "Completada" };
            string[] temas = { "Capítulo 1", "Capítulo 2", "Introducción", "Conclusión", "Resumen", "Análisis", "Ejercicios", "Problemas", "Teoría", "Práctica", "Examen", "Tarea", "Proyecto", "Investigación", "Estudio de caso" };
            string[] autores = { "García Márquez", "Cervantes", "Borges", "Cortázar", "Neruda", "Paz", "Vargas Llosa", "Allende", "Mistral", "Fuentes", "Rulfo", "Bolaño", "Poniatowska", "Sabato", "Benedetti" };

            // Obtener la lista de archivos PDF disponibles en la carpeta Uploads
            string uploadsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\..\\..\\Wirin.Api\\Uploads");
            string[] pdfFiles = Directory.GetFiles(uploadsPath, "*.pdf");

            // Para cada OrderDelivery, crear al menos 10 órdenes
            foreach (var delivery in orderDeliveries)
            {
                // Determinar cuántas órdenes crear para esta entrega (entre 10 y 15)
                int numOrders = random.Next(5, 6);
                
                for (int i = 0; i < numOrders; i++)
                {
                    // Seleccionar usuarios aleatorios para esta orden
                    var bibliotecario = bibliotecarios[random.Next(bibliotecarios.Count)];
                    var voluntario = voluntarios[random.Next(voluntarios.Count)];
                    var alumno = alumnos[random.Next(alumnos.Count)];
                    var revisor = revisores[random.Next(revisores.Count)];
                    
                    // Generar datos aleatorios para la orden
                    string nombre = $"Tarea {i+1} - {delivery.Title}";
                    string tema = temas[random.Next(temas.Length)];
                    string descripcion = $"Transcripción de {tema} para {delivery.Title}";
                    string autor = autores[random.Next(autores.Length)];
                    string rangoPaginas = $"{random.Next(1, 5)}-{random.Next(5, 20)}";
                    bool esPrioritario = random.Next(10) < 2; // 20% de probabilidad de ser prioritario
                    string estado = estados[random.Next(estados.Length)];
                    DateTime fechaCreacion = DateTime.UtcNow.AddDays(-random.Next(1, 30)); // Entre 1 y 30 días atrás
                    DateTime fechaLimite = fechaCreacion.AddDays(random.Next(5, 21)); // Entre 5 y 20 días después de la creación

                    // Asignar siempre un archivo PDF aleatorio a cada orden
                    string filePath = null;
                    if (pdfFiles.Length > 0)
                    {
                        string randomPdfFile = pdfFiles[random.Next(pdfFiles.Length)];
                        // Guardar la ruta en el formato Wirin.Api/Uploads/<nombre pdf>
                        filePath = Path.GetFileName(randomPdfFile);
                    }

                    // Crear la orden
                    orders.Add(new OrderEntity
                    {
                        Name = nombre,
                        Subject = tema,
                        Description = descripcion,
                        AuthorName = autor,
                        rangePage = rangoPaginas,
                        IsPriority = esPrioritario,
                        Status = estado,
                        CreationDate = fechaCreacion,
                        LimitDate = fechaLimite,
                        CreatedByUserId = bibliotecario.Id,
                        FilePath = filePath, // Asignar el archivo PDF aleatorio o null
                        VoluntarioId = voluntario.Id,
                        AlumnoId = alumno.Id,
                        RevisorId = revisor.Id,
                        DelivererId = delivery.Id // Asociar con la entrega correspondiente
                    });
                }
            }

            await dbContext.Orders.AddRangeAsync(orders);
            await dbContext.SaveChangesAsync();
        }
    }

    private static async Task SeedOrderSequencesAsync(WirinDbContext dbContext)
    {
        if (!dbContext.OrderSequences.Any())
        {
            var orders = dbContext.Orders.ToList();

            if (!orders.Any())
            {
                return;
            }

            var orderSequences = new List<OrderSequenceEntity>();
            var orderCounter = new Dictionary<int, int>(); // Para llevar la cuenta de órdenes por entrega

            foreach (var order in orders)
            {
                // Solo procesar órdenes que tengan un DelivererId
                if (order.DelivererId.HasValue)
                {
                    int deliveryId = order.DelivererId.Value;
                    
                    // Inicializar contador si es la primera orden para esta entrega
                    if (!orderCounter.ContainsKey(deliveryId))
                    {
                        orderCounter[deliveryId] = 1;
                    }
                    
                    // Crear la secuencia
                    var orderSequence = new OrderSequenceEntity
                    {
                        OrderId = order.Id,
                        OrderDeliveryId = deliveryId,
                        Order = orderCounter[deliveryId]
                    };
                    
                    orderSequences.Add(orderSequence);
                    
                    // Incrementar el contador para esta entrega
                    orderCounter[deliveryId]++;
                }
            }

            await dbContext.OrderSequences.AddRangeAsync(orderSequences);
            await dbContext.SaveChangesAsync();
        }
    }
}