﻿
namespace xNet.Net
{
    /// <summary>
    /// Определяет свойство объекта, используемого для работы с HTTP-протоколом и метод для его инициализации.
    /// </summary>
    /// <remarks>Данный интерфейс может пригодиться в случае, если класс использует <see cref="HttpRequest"/> для работы с HTTP-протоколом. Так как <see cref="HttpRequest"/> может содержать постоянное соединение и различные настройки, то желательно для всех запросов использовать один и тот же объект класса <see cref="HttpRequest"/>, что позволит сократить объём кода и увеличить скорость работы.</remarks>
    public interface IHttpConnect
    {
        /// <summary>
        /// Объект для отправки запросов HTTP-серверу.
        /// </summary>
        HttpRequest Request { get; set; }

        /// <summary>
        /// Инициализирует объект для отправки запросов HTTP-серверу. Здесь можно установить необходимые настройки запроса для конкретного класса.
        /// </summary>
        /// <param name="request">Объект для отправки запросов HTTP-серверу.</param>
        void InitRequest(HttpRequest request);
    }
}