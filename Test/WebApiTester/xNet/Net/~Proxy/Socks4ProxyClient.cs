using Lottery.Core.xNet;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace xNet.Net
{
    /// <summary>
    /// Представ?ет клиент для Socks4 прокси-сервер?
    /// </summary>
    public class Socks4ProxyClient : ProxyClient
    {
        #region Констант?(защищённые)

        internal protected const int DefaultPort = 1080;

        internal protected const byte VersionNumber = 4;
        internal protected const byte CommandConnect = 0x01;
        internal protected const byte CommandBind = 0x02;
        internal protected const byte CommandReplyRequestGranted = 0x5a;
        internal protected const byte CommandReplyRequestRejectedOrFailed = 0x5b;
        internal protected const byte CommandReplyRequestRejectedCannotConnectToIdentd = 0x5c;
        internal protected const byte CommandReplyRequestRejectedDifferentIdentd = 0x5d;

        #endregion


        #region Конструкторы (открытые)

        /// <summary>
        /// Инициализирует новы?экземп??класса <see cref="Socks4ProxyClient"/>.
        /// </summary>
        public Socks4ProxyClient()
            : this(null) { }

        /// <summary>
        /// Инициализирует новы?экземп??класса <see cref="Socks4ProxyClient"/> заданным хостом прокси-сервер? ?устанавливае?порт равным - 1080.
        /// </summary>
        /// <param name="host">Хост прокси-сервер?</param>
        public Socks4ProxyClient(string host)
            : this(host, DefaultPort) { }

        /// <summary>
        /// Инициализирует новы?экземп??класса <see cref="Socks4ProxyClient"/> заданным?данным??прокси-сервер?
        /// </summary>
        /// <param name="host">Хост прокси-сервер?</param>
        /// <param name="port">Порт прокси-сервер?</param>
        public Socks4ProxyClient(string host, int port)
            : this(host, port, string.Empty) { }

        /// <summary>
        /// Инициализирует новы?экземп??класса <see cref="Socks4ProxyClient"/> заданным?данным??прокси-сервер?
        /// </summary>
        /// <param name="host">Хост прокси-сервер?</param>
        /// <param name="port">Порт прокси-сервер?</param>
        /// <param name="username">Имя пользовате? для авторизаци?на прокси-сервер?</param>
        public Socks4ProxyClient(string host, int port, string username)
            : base(ProxyType.Socks4, host, port, username, null) { }

        #endregion


        #region Статически?методы (закрытые)

        /// <summary>
        /// Преобразуе?строку ?экземп??класса <see cref="Socks4ProxyClient"/>.
        /// </summary>
        /// <param name="proxyAddress">Строка вида - хост:порт:имя_пользовате?:пароль. Тр?последни?параметр?являют? необязательными.</param>
        /// <returns>Экземп??класса <see cref="Socks4ProxyClient"/>.</returns>
        /// <exception cref="System.ArgumentNullException">Значение параметр?<paramref name="proxyAddress"/> равн?<see langword="null"/>.</exception>
        /// <exception cref="System.ArgumentException">Значение параметр?<paramref name="proxyAddress"/> являет? пустой строко?</exception>
        /// <exception cref="System.FormatException">Формат порт?являет? неправильным.</exception>
        public static Socks4ProxyClient Parse(string proxyAddress)
        {
            return ProxyClient.Parse(ProxyType.Socks4, proxyAddress) as Socks4ProxyClient;
        }

        /// <summary>
        /// Преобразуе?строку ?экземп??класса <see cref="Socks4ProxyClient"/>. Возвращает значение, указывающе? успешн?ли выполнен?преобразование.
        /// </summary>
        /// <param name="proxyAddress">Строка вида - хост:порт:имя_пользовате?:пароль. Тр?последни?параметр?являют? необязательными.</param>
        /// <param name="result">Если преобразование выполнен?успешн? то содержит экземп??класса <see cref="Socks4ProxyClient"/>, инач?<see langword="null"/>.</param>
        /// <returns>Значение <see langword="true"/>, если параметр <paramref name="proxyAddress"/> преобразован успешн? инач?<see langword="false"/>.</returns>
        public static bool TryParse(string proxyAddress, out Socks4ProxyClient result)
        {
            ProxyClient proxy;

            if (ProxyClient.TryParse(ProxyType.Socks4, proxyAddress, out proxy))
            {
                result = proxy as Socks4ProxyClient;
                return true;
            }
            else
            {
                result = null;
                return false;
            }
        }

        #endregion


        /// <summary>
        /// Создаё?соединение ?сервером чере?прокси-сервер.
        /// </summary>
        /// <param name="destinationHost">Хост сервер? ?которы?нужн?связать? чере?прокси-сервер.</param>
        /// <param name="destinationPort">Порт сервер? ?которы?нужн?связать? чере?прокси-сервер.</param>
        /// <param name="tcpClient">Соединение, чере?которо?нужн?работать, ил?значение <see langword="null"/>.</param>
        /// <returns>Соединение ?сервером чере?прокси-сервер.</returns>
        /// <exception cref="System.InvalidOperationException">
        /// Значение свойства <see cref="Host"/> равн?<see langword="null"/> ил?имее?нулеву?длин?
        /// -ил?
        /// Значение свойства <see cref="Port"/> меньше 1 ил?больше 65535.
        /// -ил?
        /// Значение свойства <see cref="Username"/> имее?длин?боле?255 символов.
        /// -ил?
        /// Значение свойства <see cref="Password"/> имее?длин?боле?255 символов.
        /// </exception>
        /// <exception cref="System.ArgumentNullException">Значение параметр?<paramref name="destinationHost"/> равн?<see langword="null"/>.</exception>
        /// <exception cref="System.ArgumentException">Значение параметр?<paramref name="destinationHost"/> являет? пустой строко?</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">Значение параметр?<paramref name="destinationPort"/> меньше 1 ил?больше 65535.</exception>
        /// <exception cref="xNet.Net.ProxyException">Ошибка пр?работе ?прокси-сервером.</exception>
        public override TcpClient CreateConnection(string destinationHost, int destinationPort, TcpClient tcpClient = null)
        {
            CheckState();

            #region Проверка параметров

            if (destinationHost == null)
            {
                throw new ArgumentNullException("destinationHost");
            }

            if (destinationHost.Length == 0)
            {
                throw ExceptionHelper.EmptyString("destinationHost");
            }

            if (!ExceptionHelper.ValidateTcpPort(destinationPort))
            {
                throw ExceptionHelper.WrongTcpPort("destinationPort");
            }

            #endregion

            TcpClient curTcpClient = tcpClient;

            if (curTcpClient == null)
            {
                curTcpClient = CreateConnectionToProxy();
            }

            try
            {
                SendCommand(curTcpClient.GetStream(), CommandConnect, destinationHost, destinationPort);
            }
            catch (Exception ex)
            {
                curTcpClient.Close();

                if (ex is IOException || ex is SocketException)
                {
                    throw NewProxyException(Resources.ProxyException_Error, ex);
                }

                throw;
            }

            return curTcpClient;
        }


        #region Методы (внутренние защищённые)

        internal protected virtual void SendCommand(NetworkStream nStream, byte command, string destinationHost, int destinationPort)
        {
            byte[] dstPort = GetIPAddressBytes(destinationHost);
            byte[] dstIp = GetPortBytes(destinationPort);

            byte[] userId = string.IsNullOrEmpty(_username) ?
                new byte[0] : Encoding.ASCII.GetBytes(_username);

            // +----+----+----+----+----+----+----+----+----+----+....+----+
            // | VN | CD | DSTPORT |      DSTIP        | USERID       |NULL|
            // +----+----+----+----+----+----+----+----+----+----+....+----+
            //    1    1      2              4           variable       1
            byte[] request = new byte[9 + userId.Length];

            request[0] = VersionNumber;
            request[1] = command;
            dstIp.CopyTo(request, 2);
            dstPort.CopyTo(request, 4);
            userId.CopyTo(request, 8);
            request[8 + userId.Length] = 0x00;

            nStream.Write(request, 0, request.Length);

            // +----+----+----+----+----+----+----+----+
            // | VN | CD | DSTPORT |      DSTIP        |
            // +----+----+----+----+----+----+----+----+
            //   1    1       2              4
            byte[] response = new byte[8];

            nStream.Read(response, 0, response.Length);

            byte reply = response[1];

            // Если запрос не выполнен.
            if (reply != CommandReplyRequestGranted)
            {
                HandleCommandError(reply);
            }
        }

        internal protected byte[] GetIPAddressBytes(string destinationHost)
        {
            IPAddress ipAddr = null;

            if (!IPAddress.TryParse(destinationHost, out ipAddr))
            {
                try
                {
                    IPAddress[] ips = Dns.GetHostAddresses(destinationHost);

                    if (ips.Length > 0)
                    {
                        ipAddr = ips[0];
                    }
                }
                catch (Exception ex)
                {
                    if (ex is SocketException || ex is ArgumentException)
                    {
                        throw new ProxyException(string.Format(
                            Resources.ProxyException_FailedGetHostAddresses, destinationHost), this, ex);
                    }

                    throw;
                }
            }

            return ipAddr.GetAddressBytes();
        }

        internal protected byte[] GetPortBytes(int port)
        {
            byte[] array = new byte[2];

            array[0] = (byte)(port / 256);
            array[1] = (byte)(port % 256);

            return array;
        }

        internal protected void HandleCommandError(byte command)
        {
            string errorMessage;

            switch (command)
            {
                case CommandReplyRequestRejectedOrFailed:
                    errorMessage = Resources.Socks4_CommandReplyRequestRejectedOrFailed;
                    break;

                case CommandReplyRequestRejectedCannotConnectToIdentd:
                    errorMessage = Resources.Socks4_CommandReplyRequestRejectedCannotConnectToIdentd;
                    break;

                case CommandReplyRequestRejectedDifferentIdentd:
                    errorMessage = Resources.Socks4_CommandReplyRequestRejectedDifferentIdentd;
                    break;

                default:
                    errorMessage = Resources.Socks_UnknownError;
                    break;
            }

            string exceptionMsg = string.Format(
                Resources.ProxyException_CommandError, errorMessage, ToString());

            throw new ProxyException(exceptionMsg, this);
        }

        #endregion
    }
}