using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialCases : MonoBehaviour
{

    private static SpecialCases instance = null;

    public static SpecialCases Instance
    {
        get
        {
            return instance;
        }
    }
    private void Awake()
    {
        // if the singleton hasn't been initialized yet
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }

        instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    //Esta clase la utilizaremos para definir todas las variables que usaremos en los casos "especiales" :')

    #region Case3
    public int currentNurse = 1;
    public bool playingAnimation = false;
    #endregion

    public string ChechkAudio(string audio_id, string _speaker) 
    {
        string real_audio_id = audio_id;
        //Debug.Log($"Vamos a comprobar el audio del speaker {_speaker}");
        if (_speaker == "EndoscopyNurse")//tenemos que comprobar cual de ellas es :')
        {
            if (currentNurse == 2)
            {
                real_audio_id = audio_id + "2";
            }
            else if (currentNurse == 3)
            {
                real_audio_id = audio_id + "3"; ;
            }

            Debug.Log($"Vamos a cargar el audio {real_audio_id} de la enfermera {currentNurse}");
        }

        return real_audio_id;
    }

    public void CheckSituation(string situation_id) //SITUATION 5 AND 3
    {
        //Check if nurse cry
        if (situation_id == "S3_4" || situation_id == "S5")
        {
            currentNurse++;
            Debug.Log($"Cambiamos a la enfermera {currentNurse}");
        }
    }

    //devolver float
    public void CheckSpecialEvent(string dialogue_id)//con esto comprobaremos si en el dialogo actual debe darse alguna situacion en concreto
    {
        //adaptar para que sirva para todos los casos
        switch (dialogue_id)
        {
            case "D3.1":
                SimpleWaypointMovement movementNurse = Case3Resources.Instance.nurse.GetComponent<SimpleWaypointMovement>();
                Case3Resources.Instance.doorAnim.Play();
                movementNurse.canMove = true;
                playingAnimation = true;
                break; 
            case "D5.1":
                SimpleWaypointMovement movementNurse2 = Case3Resources.Instance.nurse.GetComponent<SimpleWaypointMovement>();
                Case3Resources.Instance.doorAnim.Play();
                movementNurse2.canMove = true;
                playingAnimation = true;
                break;
            case "D11.1": //dialogo en el que entra el secretario
                SimpleWaypointMovement movement = Case3Resources.Instance.secretary.GetComponent<SimpleWaypointMovement>();
                Case3Resources.Instance.doorAnim.Play();
                movement.canMove = true;
                playingAnimation = true;
                break;
            case "D1.4": //este es el dialogo en el que el anestesiologo se gira hacia la enfermera de endoscopia
                //
                playingAnimation = true;

                break;
        }

    }

    public void TurnsTo(GameObject characet, Vector3 targetDirection)
    {
        //activar aniamcion de girar
        //rotar transform (lookAt targetDirection)
    }
}
