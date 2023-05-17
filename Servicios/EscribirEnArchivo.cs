namespace WebApiAutores.Servicios
{
    public class EscribirEnArchivo : IHostedService
    {
        public readonly IWebHostEnvironment env;
        public readonly string nombreArchivo = "archivo 1.txt";
        private Timer timer;
        public EscribirEnArchivo(IWebHostEnvironment env)
        {
            this.env = env;
        }


        public Task StartAsync(CancellationToken cancellationToken)
        {
            timer = new Timer(DoWord, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));
            Escribir("Proceso iniciado");
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            timer.Dispose();
            Escribir("Proceso finalizado");
            return Task.CompletedTask;
        }
        private void DoWord(object state)
        {
            Escribir("Proceso en ejecucion: " + DateTime.Now.ToString("hh:mm:ss"));
        }

        private void Escribir(string mensaje)
        {
            var ruta = $@"{env.ContentRootPath}\wwwroot\{nombreArchivo}";
            using (StreamWriter writer = new StreamWriter(ruta, append: true))
            {
                writer.WriteLine(mensaje);
            }
        }
    }
}
