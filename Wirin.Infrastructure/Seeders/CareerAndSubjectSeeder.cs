using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System; 
using System.Linq;
using System.Threading.Tasks;
using Wirin.Domain.Models;
using Wirin.Infrastructure.Context;
using Wirin.Infrastructure.Entities;

namespace Wirin.Infrastructure.Seeders;

public static class CareerAndSubjectSeeder
{
    public static async Task SeedCareersAndSubjectsAsync(IServiceProvider serviceProvider)
    {
        var dbContext = serviceProvider.GetRequiredService<WirinDbContext>();
        await dbContext.Database.OpenConnectionAsync();

        if (!dbContext.Careers.Any())
        {


            var careers = new List<CareerEntity>
            {
                new CareerEntity { Id = 1, Name = "OTROS", CodDepartamento = "O" },
                new CareerEntity { Id = 55, Name = "LICENCIATURA EN ADMINISTRACION DE LA EDUCACION SUPERIOR", CodDepartamento = "F" },
                new CareerEntity { Id = 57, Name = "LICENCIATURA EN GESTION EDUCATIVA (CON ARTICULACION)", CodDepartamento = "F" },
                new CareerEntity { Id = 58, Name = "LIC. EN LENGUA Y LITERATURA", CodDepartamento = "F" },
                new CareerEntity { Id = 59, Name = "LICENCIATURA EN LENGUA Y LITERATURA (CON ARTICULACION)", CodDepartamento = "F" },
                new CareerEntity { Id = 60, Name = "LIC. EN COMERCIO INTERNACIONAL", CodDepartamento = "F" },
                new CareerEntity { Id = 61, Name = "LICENCIATURA EN COMERCIO INTERNACIONAL - CICLO DE COMPLEMENTACIÓN CURRICULAR", CodDepartamento = "F" },
                new CareerEntity { Id = 63, Name = "LICENCIATURA EN MATEMATICA APLICADA (CON ARTICULACION)", CodDepartamento = "F" },
                new CareerEntity { Id = 68, Name = "MARTILLERO, CORREDOR PÚBLICO Y TASADOR", CodDepartamento = "F" },
                new CareerEntity { Id = 69, Name = "LIC. EN EDUCACIÓN FÍSICA", CodDepartamento = "F" },
                new CareerEntity { Id = 71, Name = "LIC. EN ADMINISTRACIÓN DE SEGUROS Y RESGUARDOS", CodDepartamento = "F" },
                new CareerEntity { Id = 72, Name = "LIC. EN ADMINISTRACIÓN TRIBUTARIA", CodDepartamento = "F" },
                new CareerEntity { Id = 73, Name = "LIC. EN MATEMÁTICA APLICADA", CodDepartamento = "F" },
                new CareerEntity { Id = 74, Name = "LIC. EN HISTORIA", CodDepartamento = "F" },
                new CareerEntity { Id = 75, Name = "LIC. EN ENFERMERÍA", CodDepartamento = "F" },
                new CareerEntity { Id = 76, Name = "LOCUTOR NACIONAL", CodDepartamento = "K" },
                new CareerEntity { Id = 77, Name = "LIC. EN GESTIÓN EDUCATIVA", CodDepartamento = "F" },
                new CareerEntity { Id = 78, Name = "LIC. EN MARKETING", CodDepartamento = "F" },
                new CareerEntity { Id = 79, Name = "LICENCIATURA EN GESTION AMBIENTAL", CodDepartamento = "F" },
                new CareerEntity { Id = 80, Name = "LIC. EN GESTIÓN DE TECNOLOGÍA", CodDepartamento = "F" },
                new CareerEntity { Id = 81, Name = "LICENCIATURA EN CRIMINALISTICA", CodDepartamento = "F" },
                new CareerEntity { Id = 82, Name = "LIC. EN LENGUA INGLESA", CodDepartamento = "F" },
                new CareerEntity { Id = 83, Name = "TECNICATURA EN ADMINISTRACION BANCARIA", CodDepartamento = "F" },
                new CareerEntity { Id = 84, Name = "TECNICATURA EN GESTIÓN Y ADMINISTRACIÓN UNIVERSITARIA", CodDepartamento = "F" },
                new CareerEntity { Id = 85, Name = "TECNICATURA UNIVERSITARIA EN PSICOPEDAGOGIA", CodDepartamento = "F" },
                new CareerEntity { Id = 86, Name = "LICENCIATURA EN PSICOPEDAGOGIA - CICLO", CodDepartamento = "F" },
                new CareerEntity { Id = 87, Name = "LICENCIATURA EN GESTIÓN DE TECNOLOGÍA - CICLO DE COMPLEMENTACIÓN CURRICULAR -", CodDepartamento = "F" },
                new CareerEntity { Id = 88, Name = "TECNICATURA UNIVERSITARIA EN SEGUROS", CodDepartamento = "F" },
                new CareerEntity { Id = 89, Name = "DIPLOMATURA EN ESCRITURA ACADÉMICA", CodDepartamento = "F" },
                new CareerEntity { Id = 90, Name = "LICENCIATURA EN GESTION DE SEGUROS - CICLO DE COMPLEMENTACION CURRICULAR", CodDepartamento = "F" },
                new CareerEntity { Id = 91, Name = "LICENCIATURA EN GESTION DE SEGUROS - CICLO DE COMPLEMENTACION CURRICULAR", CodDepartamento = "F" },
                new CareerEntity { Id = 92, Name = "DIPLOMATURA EN GESTION Y COMUNICACION POLITICA", CodDepartamento = "F" },
                new CareerEntity { Id = 93, Name = "DIPLOMATURA EN SEGURIDAD CIUDADANA", CodDepartamento = "G" },
                new CareerEntity { Id = 94, Name = "LICENCIATURA EN HISTORIA CICLO DE COMPLEMENTACION CURRICULAR", CodDepartamento = "F" },
                new CareerEntity { Id = 95, Name = "LICENCIATURA EN HISTORIA CICLO DE COMPLEMENTACION CURRICULAR", CodDepartamento = "F" },
                new CareerEntity { Id = 96, Name = "LICENCIATURA EN GESTIÓN EDUCATIVA CICLO DE COMPLEMENTACIÓN CURRICULAR (CON ARTICULACIÓN)", CodDepartamento = "F" },
                new CareerEntity { Id = 97, Name = "LICENCIATURA EN GESTIÓN EDUCATIVA CICLO DE COMPLEMENTACIÓN CURRICULAR", CodDepartamento = "F" },
                new CareerEntity { Id = 98, Name = "LICENCIATURA EN GESTION TRIBUTARIA - CICLO DE COMPLEMENTACION CURRICULAR", CodDepartamento = "F" },
                new CareerEntity { Id = 99, Name = "DIPLOMATURA EN PSICOPEDAGOGIA JURIDICO FORENSE", CodDepartamento = "F" },
                new CareerEntity { Id = 101, Name = "LIC. EN ADMINISTRACIÓN", CodDepartamento = "E" },
                new CareerEntity { Id = 102, Name = "CONTADOR PÚBLICO", CodDepartamento = "E" },
                new CareerEntity { Id = 103, Name = "LIC. EN COMERCIO INTERNACIONAL", CodDepartamento = "E" },
                new CareerEntity { Id = 104, Name = "TECNICATURA EN COMERCIO EXTERIOR", CodDepartamento = "E" },
                new CareerEntity { Id = 105, Name = "LIC. EN ECONOMÍA", CodDepartamento = "E" },
                new CareerEntity { Id = 107, Name = "TECNICATURA EN CONTROL Y AUDITORIA GUBERNAMENTAL", CodDepartamento = "E" },
                new CareerEntity { Id = 201, Name = "ING. EN INFORMÁTICA", CodDepartamento = "I" },
                new CareerEntity { Id = 202, Name = "ING. EN ELECTRÓNICA", CodDepartamento = "I" },
                new CareerEntity { Id = 203, Name = "ING. INDUSTRIAL", CodDepartamento = "I" },
                new CareerEntity { Id = 204, Name = "TEC. EN ELECTRÓNICA (SONIDO Y GRABACIÓN)", CodDepartamento = "I" },
                new CareerEntity { Id = 205, Name = "TEC. EN PROCESOS INDUSTRIALES (CALZADO)", CodDepartamento = "I" },
                new CareerEntity { Id = 206, Name = "TEC. EN DESARROLLO WEB", CodDepartamento = "I" },
                new CareerEntity { Id = 207, Name = "ING. CIVIL", CodDepartamento = "I" },
                new CareerEntity { Id = 208, Name = "TEC. EN PROCESOS INDUSTRIALES (METALMECÁNICA)", CodDepartamento = "I" },
                new CareerEntity { Id = 209, Name = "ARQUITECTURA", CodDepartamento = "I" },
                new CareerEntity { Id = 210, Name = "TEC. EN DESARROLLO DE APLICACIONES P/MÓVILES", CodDepartamento = "I" },
                new CareerEntity { Id = 211, Name = "ING. MECÁNICA", CodDepartamento = "I" },
                new CareerEntity { Id = 212, Name = "INGENIERIA EN ENERGIAS", CodDepartamento = "I" },
                new CareerEntity { Id = 301, Name = "LIC. EN TRABAJO SOCIAL", CodDepartamento = "H" },
                new CareerEntity { Id = 302, Name = "LIC. EN COMUNICACIÓN SOCIAL", CodDepartamento = "H" },
                new CareerEntity { Id = 304, Name = "LIC. EN RELACIONES LABORALES", CodDepartamento = "H" },
                new CareerEntity { Id = 306, Name = "LIC. EN RELACIONES PÚBLICAS", CodDepartamento = "H" },
                new CareerEntity { Id = 307, Name = "TEC. EN CEREMONIAL Y PROTOCOLO", CodDepartamento = "H" },
                new CareerEntity { Id = 309, Name = "TECNICATURA EN RELACIONES LABORALES", CodDepartamento = "H" },
                new CareerEntity { Id = 402, Name = "LICENCIATURA EN EDUCACION FISICA", CodDepartamento = "H" },
                new CareerEntity { Id = 403, Name = "TECNICATURA DEPORTIVA", CodDepartamento = "H" },
                new CareerEntity { Id = 404, Name = "PROFESORADO EN EDUCACION FISICA", CodDepartamento = "H" },
                new CareerEntity { Id = 405, Name = "LICENCIATURA EN EDUCACION FISICA", CodDepartamento = "H" },
                new CareerEntity { Id = 601, Name = "ABOGACÍA", CodDepartamento = "D" },
                new CareerEntity { Id = 602, Name = "LIC. EN CIENCIA POLÍTICA", CodDepartamento = "D" },
                new CareerEntity { Id = 603, Name = "TEC. EN GESTIÓN PÚBLICA", CodDepartamento = "D" },
                new CareerEntity { Id = 604, Name = "PROCURADOR", CodDepartamento = "D" },
                new CareerEntity { Id = 750, Name = "LIC. EN ENFERMERIA", CodDepartamento = "S" },
                new CareerEntity { Id = 751, Name = "TECNICATURA UNIVERSITARIA EN ANATOMIA PATOLOGICA", CodDepartamento = "S" },
                new CareerEntity { Id = 752, Name = "LIC. EN NUTRICIÓN", CodDepartamento = "S" },
                new CareerEntity { Id = 754, Name = "LIC. EN KINESIOLOGÍA Y FISIATRÍA", CodDepartamento = "S" },
                new CareerEntity { Id = 756, Name = "MEDICINA", CodDepartamento = "S" },
                new CareerEntity { Id = 757, Name = "DOCTORADO EN CIENCIAS DE LA SALUD", CodDepartamento = "G" },
                new CareerEntity { Id = 760, Name = "AGENTE DE PROPAGANDA MÉDICA", CodDepartamento = "X" },
                new CareerEntity { Id = 761, Name = "DIPLOMATURA EN GESTION AMBIENTAL", CodDepartamento = "X" },
                new CareerEntity { Id = 762, Name = "DIPLOMATURA EN TRANSPORTE Y LOGISTICA", CodDepartamento = "G" },
                new CareerEntity { Id = 851, Name = "TECNICATURA UNIVERSITARIA EN ARTES ESCENICAS", CodDepartamento = "K" },
                new CareerEntity { Id = 852, Name = "TECNICATURA EN ARTES AUDIOVISUALES", CodDepartamento = "K" },
                new CareerEntity { Id = 853, Name = "TECNICATURA EN ANIMACION Y ARTE DIGITAL", CodDepartamento = "K" },
                new CareerEntity { Id = 854, Name = "TECNICATURA EN PERIODISMO DEPORTIVO INTEGRAL", CodDepartamento = "K" },
                new CareerEntity { Id = 855, Name = "TECNICATURA EN GUION AUDIOVISUAL", CodDepartamento = "K" },
                new CareerEntity { Id = 856, Name = "TECNICATURA EN DISEÑO GRAFICO Y DIGITAL", CodDepartamento = "K" },
                new CareerEntity { Id = 857, Name = "DIPLOMATURA EN INFOGRAFIA Y PERIODISMO DE DATOS", CodDepartamento = "K" },
                new CareerEntity { Id = 858, Name = "DIPLOMATURA EN PERIODISMO DE INVESTIGACION", CodDepartamento = "K" },
                new CareerEntity { Id = 859, Name = "DIPLOMATURA EN PERIODISMO AMBIENTAL", CodDepartamento = "K" },
                new CareerEntity { Id = 860, Name = "DIPLOMATURA EN DOBLAJE Y ESPAÑOL NEUTRO", CodDepartamento = "K" },
                new CareerEntity { Id = 861, Name = "LICENCIATURA EN ARTE Y GESTION CULTURAL - CICLO DE COMPLEMENTACION CURRICULAR", CodDepartamento = "K" },
                new CareerEntity { Id = 862, Name = "LICENCIATURA EN ANIMACION DIGITAL - CICLO DE COMPLEMENTACION CURRICULAR", CodDepartamento = "K" },
                new CareerEntity { Id = 863, Name = "TECNICATURA UNIVERSITARIA EN PRODUCCION DE CONTENIDOS PARA COMUNICACION", CodDepartamento = "K" },
                new CareerEntity { Id = 864, Name = "TECNICATURA UNIVERSITARIA EN DESARROLLO DE VIDEOJUEGOS", CodDepartamento = "K" },
                new CareerEntity { Id = 865, Name = "DIPLOMATURA UNIVERSITARIA EN FORMACION ACTORAL ORIENTADA A PACIENTES SIMULADOS EN PRACTICAS CLINICAS", CodDepartamento = "K" },
                new CareerEntity { Id = 901, Name = "MAESTRIA EN INFORMATICA", CodDepartamento = "G" },
                new CareerEntity { Id = 902, Name = "MAESTRIA EN CIENCIAS SOCIALES", CodDepartamento = "G" },
                new CareerEntity { Id = 903, Name = "ESPECIALIZACION EN MERCADOTECNIA", CodDepartamento = "G" },
                new CareerEntity { Id = 904, Name = "ESPECIALIZACION EN INTEGR Y POL AGROPEC", CodDepartamento = "G" },
                new CareerEntity { Id = 906, Name = "MAESTRIA EN GESTION AMBIENTAL", CodDepartamento = "G" },
                new CareerEntity { Id = 909, Name = "ESPECIALIZACION EN GESTION AMBIENTAL", CodDepartamento = "G" },
                new CareerEntity { Id = 911, Name = "MAESTRIA EN FINANZAS PUBLICAS", CodDepartamento = "G" },
                new CareerEntity { Id = 912, Name = "MAESTRIA EN PSICOANALISIS", CodDepartamento = "G" },
                new CareerEntity { Id = 913, Name = "ESPECIALIZACION EN PSICOANALISIS", CodDepartamento = "G" },
                new CareerEntity { Id = 922, Name = "MAESTRIA EN RELACIONES ECONOMICAS INTERNACIONALES", CodDepartamento = "G" },
                new CareerEntity { Id = 923, Name = "ESPECIALIZACION EN GESTION ADUANERA", CodDepartamento = "G" },
                new CareerEntity { Id = 924, Name = "ESPECIALIZACION EN PROCEDIMIENTO TRIBUTARIO Y LEY PENAL TRIBUTARIA Y PREVISIONAL", CodDepartamento = "G" },
                new CareerEntity { Id = 930, Name = "ESPECIALIZACION EN ADMINISTRACION DE JUSTICIA", CodDepartamento = "G" },
                new CareerEntity { Id = 931, Name = "MAESTRIA EN ADMINISTRACION DE JUSTICIA", CodDepartamento = "G" },
                new CareerEntity { Id = 934, Name = "ESPECIALIZACION EN DOCENCIA DE LA EDUCACION SUPERIOR", CodDepartamento = "G" },
                new CareerEntity { Id = 935, Name = "ESPECIALIZACION EN DERECHO ADMINISTRATIVO", CodDepartamento = "G" },
                new CareerEntity { Id = 936, Name = "MAESTRIA EN DERECHO ADMINISTRATIVO", CodDepartamento = "G" },
                new CareerEntity { Id = 938, Name = "ESPECIALIZACION EN ADMINISTRACION BANCARIA", CodDepartamento = "G" },
                new CareerEntity { Id = 939, Name = "MAESTRIA EN GOBERNABILIDAD", CodDepartamento = "G" },
                new CareerEntity { Id = 940, Name = "DOCTORADO EN CIENCIAS ECONOMICAS", CodDepartamento = "G" },
                new CareerEntity { Id = 942, Name = "ESPECIALIZACION EN INGRESOS PUBLICOS", CodDepartamento = "G" },
                new CareerEntity { Id = 943, Name = "MAESTRIA EN EDUCACION SUPERIOR CON MENCION EN GESTION DE LA EDUCACION SUPERIOR", CodDepartamento = "G" },
                new CareerEntity { Id = 944, Name = "MAESTRIA EN DISEÑO, GESTION Y ANALISIS DE ENCUESTAS", CodDepartamento = "G" },
                new CareerEntity { Id = 945, Name = "MAESTRIA EN COMUNICACION, CULTURA Y DISCURSOS MEDIATICOS", CodDepartamento = "G" },
                new CareerEntity { Id = 946, Name = "DOCTORADO EN CIENCIAS JURIDICAS", CodDepartamento = "G" },
                new CareerEntity { Id = 947, Name = "ESPECIALIZACION EN MAGISTRATURA", CodDepartamento = "G" },
                new CareerEntity { Id = 948, Name = "MAESTRIA EN EDUCACION A DISTANCIA", CodDepartamento = "G" },
                new CareerEntity { Id = 949, Name = "ESPECIALIZACION EN LA CUESTION MALVINAS", CodDepartamento = "G" },
                new CareerEntity { Id = 950, Name = "ESPECIALIZACION EN EDUCACION Y PROMOCION DE LA SALUD", CodDepartamento = "G" },
                new CareerEntity { Id = 951, Name = "MAESTRIA EN SALUD PUBLICA", CodDepartamento = "G" },
                new CareerEntity { Id = 952, Name = "ESPECIALIZACION EN AGROECOLOGIA", CodDepartamento = "G" },
                new CareerEntity { Id = 953, Name = "DIPLOMATURA EN GESTION DE FINANCIAMIENTO A PYMES Y MERCADO DE CAPITALES", CodDepartamento = "G" },
                new CareerEntity { Id = 954, Name = "MAESTRIA EN DESARROLLOS INFORMATICOS DE APLICACION ESPACIAL", CodDepartamento = "G" },
                new CareerEntity { Id = 955, Name = "MAESTRIA EN GESTION DE LA EDUCACION SUPERIOR", CodDepartamento = "G" },
                new CareerEntity { Id = 957, Name = "DIPLOMATURA SUPERIOR EN RESPONSABILIDAD CIVIL Y REGIMEN JURIDICO DE LOS SEGUROS", CodDepartamento = "G" },
                new CareerEntity { Id = 958, Name = "DIPLOMATURA SUPERIOR EN MAGISTRATURA", CodDepartamento = "G" },
                new CareerEntity { Id = 959, Name = "ESPECIALIZACIÓN EN CIENCIAS DE DATOS", CodDepartamento = "I" },
                new CareerEntity { Id = 960, Name = "DIPLOMATURA UNIVERSITARIA EN SIMULACIÓN CLÍNICA Y SEGURIDAD DEL PACIENTE PARA CIENCIAS DE LA SALUD", CodDepartamento = "S" },
                new CareerEntity { Id = 961, Name = "DIPLOMATURA SUPERIOR EN PROCEDIMIENTO LABORAL CAPITAL FEDERAL Y PROVINCIA DE BUENOS AIRES", CodDepartamento = "G" },
                new CareerEntity { Id = 962, Name = "DIPLOMATURA SUPERIOR INTERNACIONAL EN SISTEMAS JURIDICOS, CONTABLES, COMPLIANCE Y DELITOS COMPLEJOS", CodDepartamento = "G" },
                new CareerEntity { Id = 963, Name = "DIPLOMATURA EN GESTION Y ESTRATEGIAS IMPOSITIVAS NACIONALES", CodDepartamento = "G" },
                new CareerEntity { Id = 964, Name = "DOCTORADO EN INGENIERIA CON MENCION EN TECNOLOGIA DE LA INFORMACION", CodDepartamento = "G" },
                new CareerEntity { Id = 999, Name = "CURSO IGLU", CodDepartamento = "J" },
                new CareerEntity { Id = 1101, Name = "ODONTOLOGÍA", CodDepartamento = "O" }
            };

            await dbContext.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT Careers ON");
            await dbContext.Careers.AddRangeAsync(careers);
            await dbContext.SaveChangesAsync();
            await dbContext.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT Careers OFF");


        }

        if (!dbContext.Subjects.Any())
        {
            var subjects = new List<SubjectEntity>
            {
                new SubjectEntity { Id = 1, Name = "Introducción a la Programación", CareerId = 201 },
                new SubjectEntity { Id = 2, Name = "Algoritmos y Estructuras de Datos I", CareerId = 201 },
                new SubjectEntity { Id = 3, Name = "Matemática Discreta", CareerId = 201 },
                new SubjectEntity { Id = 4, Name = "Sistemas Operativos", CareerId = 201 },
                new SubjectEntity { Id = 5, Name = "Bases de Datos I", CareerId = 201 },
                new SubjectEntity { Id = 6, Name = "Redes de Computadoras", CareerId = 201 },
                new SubjectEntity { Id = 7, Name = "Programación Orientada a Objetos", CareerId = 201 },
                new SubjectEntity { Id = 8, Name = "Análisis de Sistemas", CareerId = 201 },
                new SubjectEntity { Id = 9, Name = "Ingeniería de Software I", CareerId = 201 },
                new SubjectEntity { Id = 10, Name = "Inteligencia Artificial", CareerId = 201 },
                new SubjectEntity { Id = 11, Name = "Seguridad Informática", CareerId = 201 },
                new SubjectEntity { Id = 12, Name = "Desarrollo Web", CareerId = 201 },
                new SubjectEntity { Id = 13, Name = "Cálculo I", CareerId = 201 },
                new SubjectEntity { Id = 14, Name = "Álgebra y Geometría Analítica", CareerId = 201 },
                new SubjectEntity { Id = 15, Name = "Física I", CareerId = 201 },
                new SubjectEntity { Id = 16, Name = "Probabilidad y Estadística", CareerId = 201 },
                new SubjectEntity { Id = 17, Name = "Investigación Operativa", CareerId = 201 },
                new SubjectEntity { Id = 18, Name = "Simulación", CareerId = 201 },
                new SubjectEntity { Id = 19, Name = "Ética y Legislación Informática", CareerId = 201 },
                new SubjectEntity { Id = 20, Name = "Seminario de Actualización Tecnológica", CareerId = 201 },
                new SubjectEntity { Id = 21, Name = "Taller de Programación", CareerId = 201 },
                new SubjectEntity { Id = 22, Name = "Estructuras de Datos II", CareerId = 201 },
                new SubjectEntity { Id = 23, Name = "Diseño de Compiladores", CareerId = 201 },
                new SubjectEntity { Id = 24, Name = "Sistemas Distribuidos", CareerId = 201 },
                new SubjectEntity { Id = 25, Name = "Minería de Datos", CareerId = 201 },
                new SubjectEntity { Id = 26, Name = "Computación Gráfica", CareerId = 201 },
                new SubjectEntity { Id = 27, Name = "Procesamiento de Lenguaje Natural", CareerId = 201 },
                new SubjectEntity { Id = 28, Name = "Robótica", CareerId = 201 },
                new SubjectEntity { Id = 29, Name = "Visión por Computadora", CareerId = 201 },
                new SubjectEntity { Id = 30, Name = "Criptografía", CareerId = 201 },
                new SubjectEntity { Id = 31, Name = "Computación en la Nube", CareerId = 201 },
                new SubjectEntity { Id = 32, Name = "Big Data", CareerId = 201 },
                new SubjectEntity { Id = 33, Name = "Internet de las Cosas (IoT)", CareerId = 201 },
                new SubjectEntity { Id = 34, Name = "Programación Móvil", CareerId = 201 },
                new SubjectEntity { Id = 35, Name = "Gestión de Proyectos de Software", CareerId = 201 },
                new SubjectEntity { Id = 36, Name = "Emprendedorismo Tecnológico", CareerId = 201 },
                new SubjectEntity { Id = 37, Name = "Arquitectura de Software", CareerId = 201 },
                new SubjectEntity { Id = 38, Name = "Testing de Software", CareerId = 201 },
                new SubjectEntity { Id = 39, Name = "DevOps", CareerId = 201 },
                new SubjectEntity { Id = 40, Name = "Experiencia de Usuario (UX)", CareerId = 201 },
                new SubjectEntity { Id = 41, Name = "Diseño de Interfaces de Usuario (UI)", CareerId = 201 },
                new SubjectEntity { Id = 42, Name = "Programación Funcional", CareerId = 201 },
                new SubjectEntity { Id = 43, Name = "Programación Lógica", CareerId = 201 },
                new SubjectEntity { Id = 44, Name = "Computación Cuántica", CareerId = 201 },
                new SubjectEntity { Id = 45, Name = "Blockchain", CareerId = 201 },
                new SubjectEntity { Id = 46, Name = "Realidad Virtual y Aumentada", CareerId = 201 },
                new SubjectEntity { Id = 47, Name = "Bioinformática", CareerId = 201 },
                new SubjectEntity { Id = 48, Name = "Neurociencia Computacional", CareerId = 201 },
                new SubjectEntity { Id = 49, Name = "Sistemas de Información Geográfica (SIG)", CareerId = 201 },
                new SubjectEntity { Id = 50, Name = "Procesamiento Digital de Señales", CareerId = 201 }
            };

            await dbContext.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT Subjects ON");
            await dbContext.Subjects.AddRangeAsync(subjects);
            await dbContext.SaveChangesAsync();
            await dbContext.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT Subjects OFF");


        }
        await dbContext.Database.CloseConnectionAsync();
    }

}