using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveController : MonoBehaviour
{
    // referencias a todos los object pools

    //referencias a todas las Waves

    // Lista que contendrá la wave actual

    private void Start()
    {
        // dentro de un while loop

        // cojo referencia a una Wave
            // Voy leyendo el tipo de Soldier que necesito y hago un GetObject (por string o typeof()?)
            // Le asigno una patrulla
            // lo agrego a una lista
            // yield return el tiempo entre spawns y spawneo un nuevo soldier
            // WaitUntil(() => todos los soldier muertos --> lista.Count = 0)
            // añado un ++ a un counter y spawneo la siguiente lista
    }
}
