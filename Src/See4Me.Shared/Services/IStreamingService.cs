using System.IO;
using System.Threading.Tasks;

namespace See4Me.Services
{
    public interface IStreamingService
    {
        ScenarioState CurrentState { get; }

        Task InitializeAsync();

        Task CleanupAsync();

        Task StartStreamingAsync(CameraPanel panel, object preview);

        Task<Stream> GetCurrentFrameAsync();

        Task StopStreamingAsync();

        CameraPanel CameraPanel { get; }

        Task SwapCameraAsync();
    }

    /// <summary>
    /// Values for identifying and controlling scenario states.
    /// </summary>
    public enum ScenarioState
    {
        /// <summary>
        /// Display is blank - default state.
        /// </summary>
        Idle,

        /// <summary>
        /// Webcam is actively engaged and a live video stream is displayed.
        /// </summary>
        Streaming
    }

    public enum CameraPanel
    {
        Unknown = 0,
        Front = 1,
        Back = 2
    }
}