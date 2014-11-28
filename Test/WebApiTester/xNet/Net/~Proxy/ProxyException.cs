using Lottery.Core.xNet;
using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace xNet.Net
{
    /// <summary>
    /// Исключение, которо?выбрасывается, ?случае возникновения ошибки пр?работе ?прокси.
    /// </summary>
    public sealed class ProxyException : NetException, ISerializable
    {
        /// <summary>
        /// Возвращает прокси-клиент, ?которо?произошл?ошибка.
        /// </summary>
        public ProxyClient ProxyClient { get; private set; }

        
        #region Конструкторы (открытые)

        /// <summary>
        /// Инициализирует новы?экземп??класса <see cref="ProxyException"/>.
        /// </summary>
        public ProxyException() : this(Resources.ProxyException_Default) { }

        /// <summary>
        /// Инициализирует новы?экземп??класса <see cref="ProxyException"/> заданным сообщением об ошибке.
        /// </summary>
        /// <param name="message">Сообщени?об ошибке ?об?снение?причин?исключен?.</param>
        /// <param name="innerException">Исключение, вызвавше?текущи?исключение, ил?значение <see langword="null"/>.</param>
        public ProxyException(string message, Exception innerException = null)
            : base(message, innerException) { }

        /// <summary>
        /// Инициализирует новы?экземп??класса <see cref="xNet.Net.ProxyException"/> заданным сообщением об ошибке ?прокси-клиентом.
        /// </summary>
        /// <param name="message">Сообщени?об ошибке ?об?снение?причин?исключен?.</param>
        /// <param name="proxyClient">Прокси-клиент, ?которо?произошл?ошибка.</param>
        /// <param name="innerException">Исключение, вызвавше?текущи?исключение, ил?значение <see langword="null"/>.</param>
        public ProxyException(string message, ProxyClient proxyClient, Exception innerException = null)
            : base(message, innerException)
        {
            ProxyClient = proxyClient;
        }

        #endregion


        /// <summary>
        /// Инициализирует новы?экземп??класса <see cref="ProxyException"/> заданным?экземп?рами <see cref="SerializationInfo"/> ?<see cref="StreamingContext"/>.
        /// </summary>
        /// <param name="serializationInfo">Экземп??класса <see cref="SerializationInfo"/>, которы?содержит сведен?, требуемы?для сериализации нового экземп?ра класса <see cref="ProxyException"/>.</param>
        /// <param name="streamingContext">Экземп??класса <see cref="StreamingContext"/>, содержащий источник сериализованного потока, связанног??новы?экземп?ро?класса <see cref="ProxyException"/>.</param>
        protected ProxyException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext) { }


        /// <summary>
        /// Заполняет экземп??<see cref="SerializationInfo"/> данным? необходимыми для сериализации исключен? <see cref="ProxyException"/>.
        /// </summary>
        /// <param name="serializationInfo">Данные ?сериализации, <see cref="SerializationInfo"/>, которы?должны использовать?.</param>
        /// <param name="streamingContext">Данные ?сериализации, <see cref="StreamingContext"/>, которы?должны использовать?.</param>
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        public override void GetObjectData(SerializationInfo serializationInfo, StreamingContext streamingContext)
        {
            base.GetObjectData(serializationInfo, streamingContext);
        }
    }
}