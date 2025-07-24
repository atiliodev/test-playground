/// Writen by Boris Chuprin smokerr@mail.ru
/// Great gratitude to everyone who helps me to convert it to C#
/// Thank you so much !!
using UnityEngine;

public class BikeSkid : MonoBehaviour
{
    public BikeController linkToBike; // Referência para o script BikeController que controla a bicicleta
    public Transform skidMarkDecal; // Prefab da marca de derrapagem
    public LayerMask allowedSurfaceLayer; // Camada de superfície permitida para deixar marcas de derrapagem
    private Vector3 lastSkidMarkPos; // Última posição da marca de derrapagem

    void Start()
    {
        linkToBike = GetComponent<BikeController>(); // Obtém a referência para o BikeController do GameObject
    }

    void FixedUpdate()
    {
        // Marca de derrapagem para a roda traseira (freagem, derrapagem)
        if (ShouldLeaveSkidMark(linkToBike.coll_rearWheel) && linkToBike.bikeSpeed > 1)
        {
            if (linkToBike.coll_rearWheel.GetGroundHit(out WheelHit hit))
            {
                if (IsAllowedSurface(hit.collider.gameObject.layer))
                {
                    CreateSkidMark(hit.point, hit.normal);
                }
            }
        }

        // Marca de derrapagem para a roda dianteira (freagem)
        if (IsFrontWheelBraking() && linkToBike.bikeSpeed > 1)
        {
            if (linkToBike.coll_frontWheel.GetGroundHit(out WheelHit hit))
            {
                if (IsAllowedSurface(hit.collider.gameObject.layer))
                {
                    CreateSkidMark(hit.point, hit.normal);
                }
            }
        }
    }

    // Verifica se a roda está derrapando e é permitido deixar a marca de derrapagem
    private bool ShouldLeaveSkidMark(WheelCollider wheel)
    {
        return wheel.sidewaysFriction.stiffness < 0.5f && wheel.GetGroundHit(out WheelHit hit);
    }

    // Verifica se a roda dianteira está freando
    private bool IsFrontWheelBraking()
    {
        return linkToBike.coll_frontWheel.brakeTorque > 0;
    }

    // Verifica se a camada da superfície está dentro da camada permitida
    private bool IsAllowedSurface(int layer)
    {
        return (allowedSurfaceLayer.value & (1 << layer)) != 0;
    }

    // Cria a marca de derrapagem na posição especificada com a rotação baseada na normal da superfície
    private void CreateSkidMark(Vector3 position, Vector3 normal)
    {
        Vector3 skidMarkPos = position + normal * 0.02f; // Posição da marca de derrapagem
        Quaternion rotation = Quaternion.LookRotation(-normal); // Rotação da marca de derrapagem

        // Criação da marca de derrapagem
        Instantiate(skidMarkDecal, skidMarkPos, rotation);
    }
}
