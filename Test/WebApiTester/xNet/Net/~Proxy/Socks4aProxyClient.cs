using System.Net.Sockets;
using System.Text;

namespace xNet.Net
{
    /// <summary>
    /// Представ?ет клиент для Socks4a прокси-сервер?
    /// </summary>
    public class Socks4aProxyClient : Socks4ProxyClient 
    {
        #region Конструкторы (открытые)

        /// <summary>
        /// Инициализирует новы?экземп??класса <see cref="Socks4aProxyClient"/>.
        /// </summary>
        public Socks4aProxyClient()
            : this(null) { }

        /// <summary>
        /// Инициализирует новы?экземп??класса <see cref="Socks4aProxyClient"/> заданным хостом прокси-сервер? ?устанавливае?порт равным - 1080.
        /// </summary>
        /// <param name="host">Хост прокси-сервер?</param>
        public Socks4aProxyClient(string host)
            : this(host, DefaultPort) { }

        /// <summary>
        /// Инициализирует новы?экземп??класса <see cref="Socks4aProxyClient"/> заданным?данным??прокси-сервер?
        /// </summary>
        /// <param name="host">Хост прокси-сервер?</param>
        /// <param name="port">Порт прокси-сервер?</param>
        public Socks4aProxyClient(string host, int port)
            : this(host, port, string.Empty) { }

        /// <summary>
        /// Инициализирует новы?экземп??класса <see cref="Socks4aProxyClient"/> заданным?данным??прокси-сервер?
        /// </summary>
        /// <param name="host">Хост прокси-сервер?</param>
        /// <param name="port">Порт прокси-сервер?</param>
        /// <param name="username">Имя пользовате? для авторизаци?на прокси-сервер?</param>
        public Socks4aProxyClient(string host, int port, string username)
            : base(host, port, username)
        {
            _type = ProxyType.Socks4a;
        }

        #endregion


        #region Методы (открытые)

        /// <summary>
        /// Преобразуе?строку ?экземп??класса <see cref="Socks4aProxyClient"/>.
        /// </summary>
        /// <param name="proxyAddress">Строка вида - хост:порт:имя_пользовате?:пароль. Тр?последни?параметр?являют? необязательными.</param>
        /// <returns>Экземп??класса <see cref="Socks4aProxyClient"/>.</returns>
        /// <exception cref="System.ArgumentNullException">Значение параметр?<paramref name="proxyAddress"/> равн?<see langword="null"/>.</exception>
        /// <exception cref="System.ArgumentException">Значение параметр?<paramref name="proxyAddress"/> являет? пустой строко?</exception>
        /// <exception cref="System.FormatException">Формат порт?являет? неправильным.</exception>
        public static Socks4aProxyClient Parse(string proxyAddress)
        {
            return ProxyClient.Parse(ProxyType.Socks4a, proxyAddress) as Socks4aProxyClient;
        }

        /// <summary>
        /// Преобразуе?строку ?экземп??класса <see cref="Socks4aProxyClient"/>. Возвращает значение, указывающе? успешн?ли выполнен?преобразование.
        /// </summary>
        /// <param name="proxyAddress">Строка вида - хост:порт:имя_пользовате?:пароль. Тр?последни?параметр?являют? необязательными.</param>
        /// <param name="result">Если преобразование выполнен?успешн? то содержит экземп??класса <see cref="Socks4aProxyClient"/>, инач?<see langword="null"/>.</param>
        /// <returns>Значение <see langword="true"/>, если параметр <paramref name="proxyAddress"/> преобразован успешн? инач?<see langword="false"/>.</returns>
        public static bool TryParse(string proxyAddress, out Socks4aProxyClient result)
        {
            ProxyClient proxy;

            if (ProxyClient.TryParse(ProxyType.Socks4a, proxyAddress, out proxy))
            {
                result = proxy as Socks4aProxyClient;
                return true;
            }
            else
            {
                result = null;
                return false;
            }
        }

        #endregion


        internal protected override void SendCommand(NetworkStream nStream, byte command, string destinationHost, int destinationPort)
        {
            byte[] dstPort = GetPortBytes(destinationPort);
            byte[] dstIp = { 0, 0, 0, 1 };

            byte[] userId = string.IsNullOrEmpty(_username) ?
                new byte[0] : Encoding.ASCII.GetBytes(_username);

            byte[] dstAddr = ASCIIEncoding.ASCII.GetBytes(destinationHost);

            // +----+----+----+----+----+----+----+----+----+----+....+----+----+----+....+----+
            // | VN | CD | DSTPORT |      DSTIP        | USERID       |NULL| DSTADDR      |NULL|
            // +----+----+----+----+----+----+----+----+----+----+....+----+----+----+....+----+
            //    1    1      2              4           variable       1    variable        1 
            byte[] request = new byte[10 + userId.Length + dstAddr.Length];

            request[0] = VersionNumber;
            request[1] = command;
            dstPort.CopyTo(request, 2);
            dstIp.CopyTo(request, 4);
            userId.CopyTo(request, 8);
            request[8 + userId.Length] = 0x00;
            dstAddr.CopyTo(request, 9 + userId.Length);
            request[9 + userId.Length + dstAddr.Length] = 0x00;

            nStream.Write(request, 0, request.Length);

            // +----+----+----+----+----+----+----+----+
            // | VN | CD | DSTPORT |      DSTIP        |
            // +----+----+----+----+----+----+----+----+
            //    1    1      2              4
            byte[] response = new byte[8];

            nStream.Read(response, 0, 8);

            byte reply = response[1];

            // Если запрос не выполнен.
            if (reply != CommandReplyRequestGranted)
            {
                HandleCommandError(reply);
            }
        }
    }
}