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
    public GameObject prop; //utilizaremos esta variable para saber que prop estamos usando, en un futuro sera un array (quiza) de momento con uno basta

    SimpleWaypointMovement movementNurse;
    string anim = "";
    Transform position; //usaremos esta variable para asignar la posicion de la mano del personaje que sea
    

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

            //Debug.Log($"Vamos a cargar el audio {real_audio_id} de la enfermera {currentNurse}");
        }

        return real_audio_id;
    }

    public void UpdateNurseName()
    {
        UI_Manager.Instance.dictionaryCharacteres["EndoscopyNurse"].gameObject.GetComponent<HandPosition>().canvasName.text = $"Endoscopy \n Nurse {currentNurse}";
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
        //TODO: 
        // test si funciona modificar el path de la enfermera para que a veces salga y a veces no
        // eliminar herramienta de la mano y lanzarla contra la enfermera
        // testear si funciona Hacer despaarecer el prop
        prop = Case3Resources.Instance.fakeProp;

        //SimpleWaypointMovement movementNurse = null;
        //adaptar para que sirva para todos los casos
        switch (dialogue_id)
        {
            
            case "D3.1": //sale llorando
                StartCoroutine(CaseD31());
                break; 
            case "D5.1": //sale llorando
                StartCoroutine(CaseD51());
                break;
            case "D11.1": //dialogo en el que entra el secretario
                StartCoroutine(CaseD111());
                break;
            case "D1.4":
                //Esta es solo para probar, no pasa realmente: //este es el dialogo en el que el anestesiologo se gira hacia la enfermera de endoscopia
                //StartCoroutine(TurnsTo(UI_Manager.Instance.dictionaryCharacteres["Anaesthesiologist"].gameObject, UI_Manager.Instance.dictionaryCharacteres["EndoscopyNurse"].gameObject.transform.position));
                break;
            case "D1.1.1":
            case "D1.1.2":
            case "D1.1.3":
            case "D1.1.4":
                //Solo de prueba: //en cualquiera de estos el anestesiologo se vuelve
                //StartCoroutine(TurnsTo(UI_Manager.Instance.dictionaryCharacteres["Anaesthesiologist"].gameObject, UI_Manager.Instance.dictionaryCharacteres["Endoscopist1"].gameObject.transform.position));
                break;

            case "D1.1": //poner endoscopio en la mano del endoescopista 1
                StartCoroutine(CaseD11());
                break;
            case "D3.2": //entra una enfermera (dividir la anim que esta hecha en dos)
                StartCoroutine(CaseD32());
                break;
            case "D5.2": //entra una enfermera (dividir la anim que esta hecha en dos)
                StartCoroutine(CaseD52());
                break;
            case "D6.2": //la enfermera te da el lazo, extraes el endoscopio y se lo das a la enfermera y la enfermera lo deja en la mesa
                StartCoroutine(CaseD62());
                break;
            case "D6.4": //rebusca en la mesa y te da una herramienta erronea
                StartCoroutine(CaseD64());
                break;
            case "D6.1.1": //lanzas la herramienta de vuelta
                StartCoroutine(CaseD611());
                break;
            case "D6.1.2": //le devuelves la herramienta
            case "D6.1.3": //le devuelves la herramienta
                StartCoroutine(CaseD613());
                break;
            case "D6.4.3": // el endoescopista 2 ayuda a la enfermera a buscar el forceps en la mesa
                StartCoroutine(CaseD643());
                break;
            case "D7.1": //la enfermera te pasa el forceps
                playingAnimation = true;
                anim = "Give";
                StartCoroutine(PlaySimpleAnim(UI_Manager.Instance.dictionaryCharacteres["EndoscopyNurse"].gameObject, anim));
                DeleteProp(prop); //igual es mejor llamar a esto desde la corutina y que desaparezca a mitad de la animacion
                break;
            case "D8.1": //la enfermera busca enrviosa la herramienta y se le cae al suelo
                playingAnimation = true;
                anim = "LookFor";
                StartCoroutine(PlaySimpleAnim(UI_Manager.Instance.dictionaryCharacteres["EndoscopyNurse"].gameObject, anim));
                //TODO: Animacion de caer al suelo
                break;
            case "D10.1": //la segunda enfermera viene y ayuda a la primera enfermera
                //playingAnimation = true;
                break;
        }
    }

    IEnumerator CaseD11()
    {
        playingAnimation = true;
        prop = GetProp("Endoscope");
        position = UI_Manager.Instance.dictionaryCharacteres["Endoscopist1"].gameObject.GetComponent<HandPosition>().handPos;
        SetProp(position, prop);// lo ponemos en tu mano
        yield return null;
        playingAnimation = false;
    }

    IEnumerator CaseD31()
    {
        playingAnimation = true;
        movementNurse = Case3Resources.Instance.nurse.GetComponent<SimpleWaypointMovement>();
        Case3Resources.Instance.doorAnim.Play();
        movementNurse.SetPathAndPlay(Case3Resources.Instance.waypointsExit);
        while (movementNurse.canMove)
        {
            yield return null; //esperamos hasta que llegue a su destino, que sera cuando el canMove sea false
        }
        playingAnimation = false;
    }

    IEnumerator CaseD32()
    {
        playingAnimation = true;
        UpdateNurseName();
        movementNurse = Case3Resources.Instance.nurse.GetComponent<SimpleWaypointMovement>();
        Case3Resources.Instance.doorAnim.Play();
        movementNurse.SetPathAndPlay(Case3Resources.Instance.waypointsEnter);
        while (movementNurse.canMove)
        {
            yield return null; //esperamos hasta que llegue a su destino, que sera cuando el canMove sea false
        }
        playingAnimation = false;
    }
    IEnumerator CaseD51() //es igual que el 31
    {
        playingAnimation = true;
        movementNurse = Case3Resources.Instance.nurse.GetComponent<SimpleWaypointMovement>();
        Case3Resources.Instance.doorAnim.Play();
        movementNurse.SetPathAndPlay(Case3Resources.Instance.waypointsExit);
        while (movementNurse.canMove)
        {
            yield return null; //esperamos hasta que llegue a su destino, que sera cuando el canMove sea false
        }
        playingAnimation = false;
    }

    IEnumerator CaseD52()
    {
        playingAnimation = true;
        UpdateNurseName();
        movementNurse = Case3Resources.Instance.nurse.GetComponent<SimpleWaypointMovement>();
        Case3Resources.Instance.doorAnim.Play();
        movementNurse.SetPathAndPlay(Case3Resources.Instance.waypointsEnter);
        while (movementNurse.canMove)
        {
            yield return null; //esperamos hasta que llegue a su destino, que sera cuando el canMove sea false
        }
        playingAnimation = false;
    }

    IEnumerator CaseD62()
    {
        playingAnimation = true;
        //Ponemos el lazo en la mano de la enfermera
        prop = GetProp("Lazo");
        position = UI_Manager.Instance.dictionaryCharacteres["EndoscopyNurse"].gameObject.GetComponent<HandPosition>().handPos;
        SetProp(position, prop);

        //Animacion de darme el lazo
        anim = "Give"; // en las de dar podemos hacer que mire a un target
        yield return PlaySimpleAnim(UI_Manager.Instance.dictionaryCharacteres["EndoscopyNurse"].gameObject, anim);

        //Ponemos el lazo en la mano del endoescopista
        position = UI_Manager.Instance.dictionaryCharacteres["Endoscopist1"].gameObject.GetComponent<HandPosition>().handPos;
        SetProp(position, prop);//lo ponemos en mi mano y lo eliminamos de la suya

        //TODO: Tenemos que quitar el lazo de tu mano para sacar el endoscopio¿?

        //TODO: extraigo endoscopio  ¿animacion?
        //Animacion de sacar el endoscopio
        anim = "EndoscopeAnim"; // en las de dar podemos ahcer que mire a un target
        yield return PlaySimpleAnim(UI_Manager.Instance.dictionaryCharacteres["Endoscopist1"].gameObject, anim);

        //Lo ponemos en tu mano
        prop = GetProp("Endoscope");
        position = UI_Manager.Instance.dictionaryCharacteres["Endoscopist1"].gameObject.GetComponent<HandPosition>().handPos;
        SetProp(position, prop);

        //Se los das a la enfermera
        anim = "Give";
        yield return PlaySimpleAnim(UI_Manager.Instance.dictionaryCharacteres["Endoscopist1"].gameObject, anim);
        position = UI_Manager.Instance.dictionaryCharacteres["EndoscopyNurse"].gameObject.GetComponent<HandPosition>().handPos;
        SetProp(position, prop);// lo ponemos en su mano

        //ir a la mesa:
        movementNurse = UI_Manager.Instance.dictionaryCharacteres["EndoscopyNurse"].gameObject.GetComponent<SimpleWaypointMovement>();
        //vementNurse.canMove = true;
        movementNurse.SetPathAndPlay(Case3Resources.Instance.waypointsToTable1);
        while (movementNurse.canMove)
        {
            yield return null; //esperamos hasta que llegue a su destino, que sera cuando el canMove sea false
        }

        // lo deja en la mesa:
        position = Case3Resources.Instance.tablePoint.transform;
        SetProp(position, prop);

        //vuelve a su sitio
        movementNurse.ResetPosition();
        while (movementNurse.canMove)
        {
            yield return null; //esperamos hasta que llegue a su destino, que sera cuando el canMove sea false
        }

        playingAnimation = false;
    }

    IEnumerator CaseD64()
    {
        //rebusca en la mesa y te da una herramienta erronea
        playingAnimation = true;

        //Ir a la mesa
        movementNurse = UI_Manager.Instance.dictionaryCharacteres["EndoscopyNurse"].gameObject.GetComponent<SimpleWaypointMovement>();
        movementNurse.SetPathAndPlay(Case3Resources.Instance.waypointsToTable1);
        while (movementNurse.canMove)
        {
            yield return null; //esperamos hasta que llegue a su destino, que sera cuando el canMove sea false
        }

        //Animacion de buscar
        anim = "LookFor";
        yield return PlaySimpleAnim(UI_Manager.Instance.dictionaryCharacteres["EndoscopyNurse"].gameObject, anim);

        //Aparecer el objeto en la mano
        position = UI_Manager.Instance.dictionaryCharacteres["EndoscopyNurse"].gameObject.GetComponent<HandPosition>().handPos;
        SetProp(position, prop);// lo ponemos en su mano

        //volver a su sitio
        movementNurse = UI_Manager.Instance.dictionaryCharacteres["EndoscopyNurse"].gameObject.GetComponent<SimpleWaypointMovement>();
        movementNurse.ResetPosition();
        while (movementNurse.canMove)
        {
            yield return null; //esperamos hasta que llegue a su destino, que sera cuando el canMove sea false
        }

        //Animacion de dar
        anim = "Give";
        yield return PlaySimpleAnim(UI_Manager.Instance.dictionaryCharacteres["EndoscopyNurse"].gameObject, anim);

        //Poner la herramienta en tu mano
        position = UI_Manager.Instance.dictionaryCharacteres["Endoscopist1"].gameObject.GetComponent<HandPosition>().handPos;
        SetProp(position, prop);// lo ponemos en tu mano

        playingAnimation = false;
    }

    IEnumerator CaseD611()
    {
        playingAnimation = true;
        anim = "Throw";
        yield return PlaySimpleAnim(UI_Manager.Instance.dictionaryCharacteres["Endoscopist1"].gameObject, anim);

        //TODO: eliminar herramienta de la mano y lanzarla contra la enfermera //bastaria con ponerle un follow point que se vaya moviendo
        float interpolationRatio = 0;
        float speed = 2f;
        Vector3 origin = prop.transform.position;
        Vector3 target = UI_Manager.Instance.dictionaryCharacteres["EndoscopyNurse"].gameObject.transform.position;
        prop.GetComponent<FollowPoint>().target = null; //de esta forma nos hara caso a nosotros y no al follow point

        //fake movement
        while (interpolationRatio < 1)
        {
            interpolationRatio += Time.deltaTime * speed;
            prop.transform.position = Vector3.Lerp(origin, target, interpolationRatio);
            yield return null;
        }

        //DeleteProp(prop); //temporal
        playingAnimation = false;
    }

    IEnumerator CaseD613()
    {
        //le devuelves la herramienta
        playingAnimation = true;

        anim = "Give";
        yield return PlaySimpleAnim(UI_Manager.Instance.dictionaryCharacteres["Endoscopist1"].gameObject, anim);

        //ir a la mesa
        movementNurse = UI_Manager.Instance.dictionaryCharacteres["EndoscopyNurse"].gameObject.GetComponent<SimpleWaypointMovement>();
        movementNurse.SetPathAndPlay(Case3Resources.Instance.waypointsToTable1);
        while (movementNurse.canMove)
        {
            yield return null; //esperamos hasta que llegue a su destino, que sera cuando el canMove sea false
        }

        // lo deja en la mesa:
        position = Case3Resources.Instance.tablePoint.transform;
        SetProp(position, prop);

        //volver a su sitio
        movementNurse = UI_Manager.Instance.dictionaryCharacteres["EndoscopyNurse"].gameObject.GetComponent<SimpleWaypointMovement>();
        movementNurse.ResetPosition();
        while (movementNurse.canMove)
        {
            yield return null; //esperamos hasta que llegue a su destino, que sera cuando el canMove sea false
        }
        //DeleteProp(prop);

        playingAnimation = false;
    }

    IEnumerator CaseD643()
    {
        // el endoescopista 2 ayuda a la enfermera a buscar el forceps en la mesa, ambas enfermera tienen que ir a la mesa
        playingAnimation = true;
        //Ir a la mesa ambos

        //ir a la mesa enfermera
        movementNurse = UI_Manager.Instance.dictionaryCharacteres["EndoscopyNurse"].gameObject.GetComponent<SimpleWaypointMovement>();
        movementNurse.SetPathAndPlay(Case3Resources.Instance.waypointsToTable1);
        while (movementNurse.canMove) //este while igual nos lo podemos machacar en este contexto, y que se quede esperando en el siguiente
        {
            yield return null; //esperamos hasta que llegue a su destino, que sera cuando el canMove sea false
        }

        //ir a la mesa endoescopista 2
        movementNurse = UI_Manager.Instance.dictionaryCharacteres["Endoscopist2"].gameObject.GetComponent<SimpleWaypointMovement>();
        movementNurse.SetPathAndPlay(Case3Resources.Instance.waypointsToTable1);
        while (movementNurse.canMove)
        {
            yield return null; //esperamos hasta que llegue a su destino, que sera cuando el canMove sea false
        }

        //Anim de buscar:
        anim = "LookFor";
        yield return PlaySimpleAnim(UI_Manager.Instance.dictionaryCharacteres["EndoscopyNurse"].gameObject, anim);

        yield return PlaySimpleAnim(UI_Manager.Instance.dictionaryCharacteres["Endoscopist2"].gameObject, anim);

        //volver a sus sitios
        movementNurse = UI_Manager.Instance.dictionaryCharacteres["EndoscopyNurse"].gameObject.GetComponent<SimpleWaypointMovement>();
        movementNurse.ResetPosition();
        while (movementNurse.canMove)
        {
            yield return null; //esperamos hasta que llegue a su destino, que sera cuando el canMove sea false
        }

        movementNurse = UI_Manager.Instance.dictionaryCharacteres["Endoscopist2"].gameObject.GetComponent<SimpleWaypointMovement>();
        movementNurse.ResetPosition();
        while (movementNurse.canMove)
        {
            yield return null; //esperamos hasta que llegue a su destino, que sera cuando el canMove sea false
        }

        //poner el prop en la mano de la enfermera
        position = UI_Manager.Instance.dictionaryCharacteres["EndoscopyNurse"].gameObject.GetComponent<HandPosition>().handPos;
        SetProp(position, prop);

        playingAnimation = false; ;
    }

    IEnumerator CaseD111()
    {
        playingAnimation = true;
        SimpleWaypointMovement movement = Case3Resources.Instance.secretary.GetComponent<SimpleWaypointMovement>();
        Case3Resources.Instance.doorAnim.Play();
        movement.canMove = true;
        while (movement.canMove)
        {
            yield return null; //esperamos hasta que llegue a su destino, que sera cuando el canMove sea false
        }
        playingAnimation = false;
    }

    IEnumerator TurnsTo(GameObject characer, Vector3 targetPosition) //para volver a la posicion original basta con indicar que se giren hacia en endoescopista 1
    {
        playingAnimation = true;
        //activar aniamcion de girar (hay que comprobar hacia donde gira y en funcion de eso poner derecha o izquierda)
        Animator animator = characer.GetComponent<Animator>();
        float turnAnimDuration = 0f;
        float rotationSpeed = 1f;
        if (targetPosition.x < characer.transform.position.x && changed(characer.transform.position.y, targetPosition.y))
        {
            Debug.Log("Movemos a la derecha");
            //animator.SetBool("turnRight", true);
            //turnAnimDuration = GetDuration("RightTurn", animator);
        }
        else if (changed(characer.transform.position.y, targetPosition.y))
        {
            Debug.Log("Movemos a la izquierda");
            //animator.SetBool("turnLeft", true);
            //turnAnimDuration = GetDuration("LeftTurn", animator);
        }

        Debug.Log($"Almacenamos la duracion de la animación actual de girarase = {turnAnimDuration}");

        //Opcion 1 (con animacion haciendo cosas raras)
        //yield return new WaitForSeconds(turnAnimDuration);
        //characer.transform.LookAt(targetPosition);


        //Opcion 2 (sin animacion)
        while (Vector3.Distance(characer.transform.forward, targetPosition - characer.transform.position) > 0.2f)//characer.transform.rotation != Quaternion.LookRotation(targetPosition - characer.transform.position)
        {
            Debug.Log($"Seguimos girando hasta que encontremos al player");
            characer.transform.rotation = Quaternion.Lerp(characer.transform.rotation, Quaternion.LookRotation(targetPosition - characer.transform.position), Time.deltaTime * rotationSpeed);
            yield return null;
        }

        playingAnimation = false;
    }

    IEnumerator PlaySimpleAnim(GameObject character, string animName)
    {
        playingAnimation = true;
        //IMPORTANTE: La animacion y la transicion deben de tener el mismo nombre para que funcione bien
        //Si se queda en bucle la animacion revisar si se ha añadido el animManager y el false a la animacion dentro del mismo 
        Animator animator = character.GetComponent<Animator>();
        animator.SetBool(animName, true);
        float animDuration = GetDuration(animName, animator);

        yield return new WaitForSeconds(animDuration);
    }

    public float GetDuration(string animationName, Animator anim)
    {
        float duration = 0f;

        AnimationClip[] clips = anim.runtimeAnimatorController.animationClips;
        foreach (AnimationClip clip in clips)
        {
            if (animationName == clip.name)
            {
                duration = clip.length;
                break;
            }
        }

        return duration;
    }

    bool changed(float a, float b)
    {
        if ((int)a == (int)b)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void SetProp(Transform coord, GameObject prop)
    {
        prop.SetActive(true);
        prop.GetComponent<FollowPoint>().target = coord;
    }

    public void DeleteProp(GameObject prop)
    {
        prop.SetActive(false);
    }


    public GameObject GetProp(string propName)
    {
        return prop;//TODO: esto debera ser un buscar en un array de posibles herramientaas, de momento solo hay una
    }



}
