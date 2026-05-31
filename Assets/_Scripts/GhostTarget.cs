using UnityEngine;
using UnityEngine.Events; // Hocanın istediği Event sistemi için bu şart

public class GhostTarget : MonoBehaviour
{
    [Header("Işın Çarpınca Ne Olsun?")]
    public UnityEvent onHit; 
}