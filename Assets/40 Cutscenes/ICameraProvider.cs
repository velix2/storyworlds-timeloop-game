using Unity.Cinemachine;

public interface ICameraProvider
{
    CinemachineCamera GetCamera(string id);
}