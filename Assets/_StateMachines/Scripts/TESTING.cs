using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TESTING : MonoBehaviour
{
    private PlayerCharacter playerCharacter;

    private void Awake()
    {
        playerCharacter = FindObjectOfType<PlayerCharacter>();


            //Debug.Log(StaticRefs.MASK_FLOOR);
            //Debug.Log(StaticRefs.MASK_GROUND);
            //Debug.Log(StaticRefs.MASK_PLATFORM);
            //Debug.Log(StaticRefs.MASK_WATER);
            //Debug.Log(StaticRefs.MASK_ENEMY);
            //Debug.Log(StaticRefs.MASK_PLAYER);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            playerCharacter.Attributes.TakeDamage(5);
        }
        else if (Input.GetKeyDown(KeyCode.P))
        {
            playerCharacter.Attributes.Alive = true;
        }



        //Debug.Log(
        //    "ground smoothing: " + playerCharacter.Attributes.GroundSpeedSmoothing +
        //    "air smoothing: " + playerCharacter.Attributes.AirSpeedSmoothing);
        //Debug.Log("velocity: " + playerCharacter.Controller.Velocity);

    }
}
