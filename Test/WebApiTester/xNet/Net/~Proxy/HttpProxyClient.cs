using Lottery.Core.xNet;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using xNet.Text;

namespace xNet.Net
{
    /// <summary>
    /// 
    /// </summary>
    public class HttpProxyClient : ProxyClient
    {
        #region ЛЅУРіЈБї

        private const int BufferSize = 50;
        private const int DefaultPort = 8080;

        #endregion


        #region Ѕб№№

        /// <summary>
        /// Инициализирует новы?экземп??класса <see cref="HttpProxyClient"/>.
        /// </summary>
        public HttpProxyClient()
            : this(null) { }

        /// <summary>
        /// Инициализирует новы?экземп??класса <see cref="HttpProxyClient"/> заданным хостом прокси-сервер? ?устанавливае?порт равным - 8080.
        /// </summary>
        /// <param name="host">Хост прокси-сервер?</param>
        public HttpProxyClient(string host)
            : this(host, DefaultPort) { }

        /// <summary>
        /// Инициализирует новы?экземп??класса <see cref="HttpProxyClient"/> заданным?данным??прокси-сервер?
        /// </summary>
        /// <param name="host">Хост прокси-сервер?</param>
        /// <param name="port">Порт прокси-сервер?</param>
        public HttpProxyClient(string host, int port)
            : this(host, port, string.Empty, string.Empty) { }

        /// <summary>
        /// Инициализирует новы?экземп??класса <see cref="HttpProxyClient"/> заданным?данным??прокси-сервер?
        /// </summary>
        /// <param name="host">Хост прокси-сервер?</param>
        /// <param name="port">Порт прокси-сервер?</param>
        /// <param name="username">Имя пользовате? для авторизаци?на прокси-сервер?</param>
        /// <param name="password">Пароль для авторизаци?на прокси-сервер?</param>
        public HttpProxyClient(string host, int port, string username, string password)
            : base(ProxyType.Http, host, port, username, password) { }

        #endregion


        #region Parse

        /// <summary>
        /// Преобразуе?строку ?экземп??класса <see cref="HttpProxyClient"/>.
        /// </summary>
        /// <param name="proxyAddress">Строка вида - хост:порт:имя_пользовате?:пароль. Тр?последни?параметр?являют? необязательными.</param>
        /// <returns>Экземп??класса <see cref="HttpProxyClient"/>.</returns>
        /// <exception cref="System.ArgumentNullException">Значение параметр?<paramref name="proxyAddress"/> равн?<see langword="null"/>.</exception>
        /// <exception cref="System.ArgumentException">Значение параметр?<paramref name="proxyAddress"/> являет? пустой строко?</exception>
        /// <exception cref="System.FormatException">Формат порт?являет? неправильным.</exception>
        public static HttpProxyClient Parse(string proxyAddress)
        {
            return ProxyClient.Parse(ProxyType.Http, proxyAddress) as HttpProxyClient;
        }

        /// <summary>
        /// Преобразуе?строку ?экземп??класса <see cref="HttpProxyClient"/>. Возвращает значение, указывающе? успешн?ли выполнен?преобразование.
        /// </summary>
        /// <param name="proxyAddress">Строка вида - хост:порт:имя_пользовате?:пароль. Тр?последни?параметр?являют? необязательными.</param>
        /// <param name="result">Если преобразование выполнен?успешн? то содержит экземп??класса <see cref="HttpProxyClient"/>, инач?<see langword="null"/>.</param>
        /// <returns>Значение <see langword="true"/>, если параметр <paramref name="proxyAddress"/> преобразован успешн? инач?<see langword="false"/>.</returns>
        public static bool TryParse(string proxyAddress, out HttpProxyClient result)
        {
            ProxyClient proxy;

            if (ProxyClient.TryParse(ProxyType.Http, proxyAddress, out proxy))
            {
                result = proxy as HttpProxyClient;
                return true;
            }
            else
            {
                result = null;
                return false;
            }
        }

        #endregion


        #region Create Connection

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
        /// <remarks>Если порт сервер?нераве?80, то для подключения использует? мето?'CONNECT'.</remarks>
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

            if (destinationPort != 80)
            {
                HttpStatusCode statusCode = HttpStatusCode.OK;

                try
                {
                    NetworkStream nStream = curTcpClient.GetStream();

                    SendConnectionCommand(nStream, destinationHost, destinationPort);
                    statusCode = ReceiveResponse(nStream);
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

                if (statusCode != HttpStatusCode.OK)
                {
                    curTcpClient.Close();

                    throw new ProxyException(string.Format(
                        Resources.ProxyException_ReceivedWrongStatusCode, statusCode, ToString()), this);
                }
            }

            return curTcpClient;
        }

        #endregion


        #region Authorization Header

        private string GenerateAuthorizationHeader()
        {
            if (!string.IsNullOrEmpty(_username) || !string.IsNullOrEmpty(_password))
            {
                string data = Convert.ToBase64String(Encoding.UTF8.GetBytes(
                    string.Format("{0}:{1}", _username, _password)));

                return string.Format("Proxy-Authorization: Basic {0}\r\n", data);
            }

            return string.Empty;
        }

        private void SendConnectionCommand(NetworkStream nStream, string destinationHost, int destinationPort)
        {
            var commandBuilder = new StringBuilder();

            commandBuilder.AppendFormat("CONNECT {0}:{1} HTTP/1.1\r\n", destinationHost, destinationPort);
            commandBuilder.AppendFormat(GenerateAuthorizationHeader());
            commandBuilder.AppendLine();

            byte[] buffer = Encoding.ASCII.GetBytes(commandBuilder.ToString());

            nStream.Write(buffer, 0, buffer.Length);
        }

        private HttpStatusCode ReceiveResponse(NetworkStream nStream)
        {
            byte[] buffer = new byte[BufferSize];
            var responseBuilder = new StringBuilder();

            WaitData(nStream);

            do
            {
                int bytesRead = nStream.Read(buffer, 0, BufferSize);
                responseBuilder.Append(Encoding.ASCII.GetString(buffer, 0, bytesRead));
            } while (nStream.DataAvailable);

            string response = responseBuilder.ToString();

            if (response.Length == 0)
            {
                throw NewProxyException(Resources.ProxyException_ReceivedEmptyResponse);
            }

            // Выде?ем строку статус? Пример: HTTP/1.1 200 OK\r\n
            string strStatus = response.Substring(" ", HttpHelper.NewLine);

            int simPos = strStatus.IndexOf(' ');

            if (simPos == -1)
            {
                throw NewProxyException(Resources.ProxyException_ReceivedWrongResponse);
            }

            string statusLine = strStatus.Substring(0, simPos);

            if (statusLine.Length == 0)
            {
                throw NewProxyException(Resources.ProxyException_ReceivedWrongResponse);
            }

            HttpStatusCode statusCode = (HttpStatusCode)Enum.Parse(
                typeof(HttpStatusCode), statusLine);

            return statusCode;
        }

        private void WaitData(NetworkStream nStream)
        {
            int sleepTime = 0;
            int delay = (nStream.ReadTimeout < 10) ?
                10 : nStream.ReadTimeout;

            while (!nStream.DataAvailable)
            {
                if (sleepTime >= delay)
                {
                    throw NewProxyException(Resources.ProxyException_WaitDataTimeout);
                }

                sleepTime += 10;
                Thread.Sleep(10);
            }
        }

        #endregion
    }
}