using System;
using System.Collections; // Необходимо для корутин
using System.Text;       // Необходимо для Encoding

using UnityEngine;
using UnityEngine.Networking; // Основное пространство имен для UnityWebRequest
namespace StateEngine.Components.Web
{
    public class RestApi : MonoBehaviour
    {
        [Tooltip("Базовый URL вашего REST API (например, https://api.example.com/v1)")]
        public string baseUrl = "YOUR_API_BASE_URL"; // <-- !!! ЗАМЕНИТЕ НА ВАШ URL !!!

        // --- Публичные методы для вызова API ---

        /// <summary>
        /// Отправляет GET запрос.
        /// </summary>
        /// <param name="endpoint">Конечная точка API (например, "/users").</param>
        /// <param name="onSuccess">Callback при успешном выполнении (статус 2xx). Получает UnityWebRequest с ответом.</param>
        /// <param name="onError">Callback при ошибке. Получает UnityWebRequest и строку с описанием ошибки.</param>
        public void Get(string endpoint, Action<UnityWebRequest> onSuccess, Action<UnityWebRequest, string> onError)
        {
            string url = baseUrl + endpoint;
            StartCoroutine(SendRequestCoroutine(UnityWebRequest.Get(url), onSuccess, onError));
        }

        /// <summary>
        /// Отправляет POST запрос с JSON данными.
        /// </summary>
        /// <param name="endpoint">Конечная точка API.</param>
        /// <param name="jsonData">Данные в формате JSON (строка).</param>
        /// <param name="onSuccess">Callback при успехе.</param>
        /// <param name="onError">Callback при ошибке.</param>
        public void Post(string endpoint, string jsonData, Action<UnityWebRequest> onSuccess, Action<UnityWebRequest, string> onError)
        {
            string url = baseUrl + endpoint;
            // Для отправки Raw JSON используем POST с ручной настройкой UploadHandler
            var request = new UnityWebRequest(url, UnityWebRequest.kHttpVerbPOST); // Используем "POST" или UnityWebRequest.kHttpVerbPOST
            SetupJsonRequest(request, jsonData);
            StartCoroutine(SendRequestCoroutine(request, onSuccess, onError));
        }

        /// <summary>
        /// Отправляет PUT запрос с JSON данными.
        /// </summary>
        /// <param name="endpoint">Конечная точка API.</param>
        /// <param name="jsonData">Данные в формате JSON (строка).</param>
        /// <param name="onSuccess">Callback при успехе.</param>
        /// <param name="onError">Callback при ошибке.</param>
        public void Put(string endpoint, string jsonData, Action<UnityWebRequest> onSuccess, Action<UnityWebRequest, string> onError)
        {
            string url = baseUrl + endpoint;
            var request = new UnityWebRequest(url, UnityWebRequest.kHttpVerbPUT); // Используем "PUT" или UnityWebRequest.kHttpVerbPUT
            SetupJsonRequest(request, jsonData);
            StartCoroutine(SendRequestCoroutine(request, onSuccess, onError));
        }

        /// <summary>
        /// Отправляет DELETE запрос.
        /// </summary>
        /// <param name="endpoint">Конечная точка API.</param>
        /// <param name="onSuccess">Callback при успехе.</param>
        /// <param name="onError">Callback при ошибке.</param>
        public void Delete(string endpoint, Action<UnityWebRequest> onSuccess, Action<UnityWebRequest, string> onError)
        {
            string url = baseUrl + endpoint;
            // UnityWebRequest.Delete() по умолчанию не устанавливает DownloadHandler,
            // добавим его, если API может вернуть тело ответа при DELETE
            var request = UnityWebRequest.Delete(url);
            request.downloadHandler = new DownloadHandlerBuffer(); // Чтобы получить тело ответа, если оно есть
            StartCoroutine(SendRequestCoroutine(request, onSuccess, onError));
        }

        // --- Вспомогательные методы и корутины ---

        /// <summary>
        /// Настраивает заголовки и тело для JSON запроса (POST/PUT).
        /// </summary>
        virtual protected void SetupJsonRequest(UnityWebRequest request, string jsonData)
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer(); // Обязательно для получения ответа
            request.SetRequestHeader("Content-Type", "application/json; charset=utf-8");
            // Можно добавить и другие общие заголовки здесь или в SetupRequest
        }

        /// <summary>
        /// Корутина для отправки запроса и обработки ответа.
        /// </summary>
        private IEnumerator SendRequestCoroutine(UnityWebRequest request, Action<UnityWebRequest> onSuccess, Action<UnityWebRequest, string> onError)
        {
            // Здесь можно добавить общие заголовки (например, авторизации) перед отправкой
            SetupRequest(request);

            Debug.Log($"[RestApi] Sending {request.method} to: {request.url}");
            if (request.uploadHandler != null && request.uploadHandler.data != null)
            {
                // Осторожно: не логируйте чувствительные данные в продакшене!
                // Debug.Log($"[RestApi] With data: {Encoding.UTF8.GetString(request.uploadHandler.data)}");
            }

            // Отправляем запрос и ждем ответа
            yield return request.SendWebRequest();

            string errorMessage = null;

            // Обработка результата запроса (с Unity 2020.1+)
            switch (request.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                    errorMessage = $"Connection Error: {request.error}";
                    break;
                case UnityWebRequest.Result.DataProcessingError:
                    errorMessage = $"Data Processing Error: {request.error}";
                    break;
                case UnityWebRequest.Result.ProtocolError: // Ошибка HTTP (например, 404, 500)
                    errorMessage = $"HTTP Error: {request.responseCode} - {request.error}";
                    // Добавляем тело ответа к сообщению об ошибке, если оно есть
                    if (request.downloadHandler != null && !string.IsNullOrEmpty(request.downloadHandler.text))
                    {
                        errorMessage += $"\nResponse Body: {request.downloadHandler.text}";
                    }
                    break;
                case UnityWebRequest.Result.Success: // Успех (статус код 2xx)
                    Debug.Log($"[RestApi] Request finished Successfully! Status Code: {request.responseCode}\nResponse: {request.downloadHandler?.text ?? "No Content"}");
                    onSuccess?.Invoke(request);
                    break;
            }

            // Если была ошибка, вызываем onError
            if (errorMessage != null)
            {
                Debug.LogError($"[RestApi] Request Failed! {errorMessage}");
                onError?.Invoke(request, errorMessage);
            }

            // Освобождаем ресурсы запроса
            request.Dispose();
        }

        /// <summary>
        /// Место для добавления общих настроек ко всем запросам (например, заголовки авторизации).
        /// </summary>
        /// <param name="request">Объект запроса.</param>
        virtual protected void SetupRequest(UnityWebRequest request)
        {
            // Установка таймаута (в секундах)
            // request.timeout = 10;

            // Добавление общих заголовков (например, для авторизации)
            // string authToken = GetAuthToken(); // Получите токен из сохраненного места
            // if (!string.IsNullOrEmpty(authToken))
            // {
            //     request.SetRequestHeader("Authorization", $"Bearer {authToken}");
            // }

            // Можно добавить другие заголовки
            // request.SetRequestHeader("Accept", "application/json");
        }
    }
}