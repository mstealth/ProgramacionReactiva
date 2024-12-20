using System;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace ProgramacionReactiva
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            //Simulación de dos sensores de temperatura
            var sensor1 = Observable.Interval(TimeSpan.FromSeconds(1))
                .Select(i => new Random().Next(15, 40)); // Temperaturas entre 15 y 40 °C

            var sensor2 = Observable.Interval(TimeSpan.FromSeconds(1))
                .Select(i => new Random().Next(10, 35)); // Temperaturas entre 10 y 35 °C

            //1. Filtrar temperaturas anormales (> 30 °C)
            var temperaturasAnormales = sensor1.Where(temp => temp > 30);

            //2. Transformar las temperaturas a un formato legible
            var temperaturasFormateadas = temperaturasAnormales.Select(temp => $"Temperatura alta detectada: {temp}°C");

            //3. Combinar datos de ambos sensores
            var combinacionSensores = sensor1.CombineLatest(sensor2, (temp1, temp2) =>
            $"Sensor 1: {temp1}°C, Sensor 2: {temp2}°C");

            //4. Agrupar temperaturas en intervalos de 5 segundos y calcular el promedio
            var temperaturasAgrupadas = sensor1
                .Buffer(TimeSpan.FromSeconds(5)) //Agrupa las emisiones en intervalos de 5 segundos
                .Select(buffer => buffer.Count > 0 //Verifica que el grupo no esté vacío
                    ? $"Promedio de los últimos 5 segundos: {buffer.Select(temp => (double)temp).Average():F2}°C"
                    : "No hay datos en este intervalo.");

            //5. Suscribirse a los flujos
            var subscription1 = temperaturasFormateadas.Subscribe(Console.WriteLine);
            var subscription2 = combinacionSensores.Subscribe(Console.WriteLine);
            var subscription3 = temperaturasAgrupadas.Subscribe(Console.WriteLine);

            //Ejecutar durante 20 segundos y luego finalizar
            await Task.Delay(TimeSpan.FromSeconds(20));

            //Cancelar las subscripciones
            subscription1.Dispose();
            subscription2.Dispose();    
            subscription3.Dispose();
        }
    }
}

//Salida:

//Sensor 1: 23°C, Sensor 2: 15°C
//Sensor 1: 23°C, Sensor 2: 23°C
//Sensor 1: 28°C, Sensor 2: 23°C
//Sensor 1: 28°C, Sensor 2: 16°C
//Temperatura alta detectada: 37°C
//Sensor 1: 21°C, Sensor 2: 16°C
//Sensor 1: 38°C, Sensor 2: 16°C
//Sensor 1: 38°C, Sensor 2: 23°C
//Sensor 1: 38°C, Sensor 2: 13°C
//Sensor 1: 15°C, Sensor 2: 13°C
//Promedio de los últimos 5 segundos: 23,00°C
//Sensor 1: 16°C, Sensor 2: 13°C
//Sensor 1: 16°C, Sensor 2: 19°C
//Sensor 1: 16°C, Sensor 2: 29°C
//Sensor 1: 36°C, Sensor 2: 29°C