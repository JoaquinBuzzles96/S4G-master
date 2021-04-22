using System;
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
    SimpleWaypointMovement endoscopist2Mov;
    string anim = "";
    Transform position; //usaremos esta variable para asignar la posicion de la mano del personaje que sea

    public string currentTool = "";
    public string nurseTool = "";
    bool isExtraNurse = false;

    #endregion

    public string ChechkAudio(string audio_id, string _speaker) 
    {
        string real_audio_id = audio_id;
        if (UI_Manager.Instance.currentCase == Cases.Case3)
        {
            //Debug.Log($"Vamos a comprobar el audio del speaker {_speaker}");
            if (_speaker == "EndoscopyNurse")//tenemos que comprobar cual de ellas es :')
            {
                if (currentNurse == 2)
                {
                    real_audio_id = audio_id + "2";
                }
                else if (currentNurse == 3)
                {
                    real_audio_id = audio_id + "3";
                }

                //Debug.Log($"Vamos a cargar el audio {real_audio_id} de la enfermera {currentNurse}");
            }
        }

        return real_audio_id;
    }

    public void UpdateNurseName()
    {
        Case3Resources.Instance.mask.SetActive(true);
        Debug.Log($"currentNurse = {currentNurse}");
        UI_Manager.Instance.dictionaryCharacteres["EndoscopyNurse"].gameObject.GetComponent<HandPosition>().canvasName.text = $"Endoscopy \n Nurse {currentNurse}";
        UI_Manager.Instance.dictionaryCharacteres["EndoscopyNurseExtra"].gameObject.GetComponent<HandPosition>().canvasName.text = $"Endoscopy \n Nurse {currentNurse + 1}";
    }

    public void CheckSituation(string situation_id) //SITUATION 5 AND 3
    {
        if (UI_Manager.Instance.currentCase == Cases.Case3)
        {
            //Check if nurse cry
            if (situation_id == "S3_4" || situation_id == "S5_6")
            {
                currentNurse++;
                Debug.Log($"Cambiamos a la enfermera {currentNurse}");
            }
        }

    }


    public void CheckSpecialEventCase3(string dialogue_id)
    {
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
            case "D1.1": //poner endoscopio en la mano del endoescopista 1
                StartCoroutine(CaseD11());
                break;
            case "D1.1.2": //La enfermera se pone la mascarilla
            case "D1.5": //La enfermera se pone la mascarilla
            case "D1.8": //La enfermera se pone la mascarilla
                StartCoroutine(CaseD112());
                break;
            case "D3.2": //entra una enfermera
                StartCoroutine(CaseD32());
                break;
            case "D4.3": //la enfermera coge una herramienta de la mesa y pregunta si es correcta (inyector)
                StartCoroutine(CaseD43());
                break;
            case "D5.2": //entra una enfermera
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
               // De momento lo quitamos, podemos poner que no te la llegue a dar --> StartCoroutine(CaseD613());
                break;
            case "D6.4.3": // el endoescopista 2 ayuda a la enfermera a buscar el forceps en la mesa
                StartCoroutine(CaseD643());
                break;
            case "D6.4.0": // el endoescopista 2  se gira hacia la enfermera
                StartCoroutine(CaseD640());
                break;
            case "D6.5": // el endoescopista 2 vuelve a su posicion inicial
                StartCoroutine(CaseD65());
                break;
            case "D7.1": //la enfermera te pasa el forceps
                StartCoroutine(CaseD71());
                break;
            case "D8.1": //la enfermera busca nerviosa la herramienta y se le cae al suelo
                StartCoroutine(CaseD81());
                break;
            case "D8.2": //la enfermera vuelve a su sitio
                StartCoroutine(CaseD82());
                break;
            case "D10.1": //la segunda enfermera viene y ayuda a la primera enfermera
                //playingAnimation = true;
                StartCoroutine(CaseD101());
                break;
        }
    }

    //Devolver float
    public void CheckSpecialEvent(string dialogue_id)//con esto comprobaremos si en el dialogo actual debe darse alguna situacion en concreto
    {
        if (UI_Manager.Instance.currentCase == Cases.Case3)
        {
            CheckSpecialEventCase3(dialogue_id);
        }
        else if (UI_Manager.Instance.currentCase == Cases.Case5)
        {
            CheckSpecialEventCase5(dialogue_id);
        }
        else if (UI_Manager.Instance.currentCase == Cases.Case9)
        {
            CheckSpecialEventCase9(dialogue_id);
        }
        else if (UI_Manager.Instance.currentCase == Cases.Case6)
        {
            CheckSpecialEventCase6(dialogue_id);
        }
        else if (UI_Manager.Instance.currentCase == Cases.Case7)
        {
            CheckSpecialEventCase7(dialogue_id);
        }
    }


    public void CheckSpecialEventCase5(string dialogue_id)
    {
        switch (dialogue_id)
        {
            case "D0.1": //es el anterior al D.1
                StartCoroutine(Case5D01());
                break;
            case "D1": //se vuelve a su sitio
                //StartCoroutine(Case5D1()); //Mejor lo hace todo en el anterior
                break;
            case "D16.1.3":
                StartCoroutine(Case5D1613());
                break;
            case "D16.2.3":
                StartCoroutine(Case5D1623());
                break;
            case "D16.3.3":
                StartCoroutine(Case5D1633());
                break;
            case "D16.4.1":
                StartCoroutine(Case5D1641());
                break;
            case "D18.1.2":
                StartCoroutine(Case5D1812());
                break;
            case "D18.2.2":
                StartCoroutine(Case5D1822());
                break;
            case "D18.3.2":
                StartCoroutine(Case5D1832());
                break;

        }
    }

    public void CheckSpecialEventCase6(string dialogue_id)
    {
        switch (dialogue_id)
        {
            case "D2.4.2":
            case "D2.2.2":
            case "D2.1.1":
            case "D2.3.2":
                StartCoroutine(Case6D2_Count_towels()); 
                break;
            case "D4.2.2":
            case "D4.3.2":
            case "D4.4.1":
                StartCoroutine(Case6D4_search_in_floor());
                break;
            case "D6.1.1":
            case "D6.1.2":
            case "D6.1.3":
            case "D6.2.1":
            case "D6.2.2":
            case "D6.2.3":
            case "D6.3.1":
            case "D6.3.2":
            case "D6.3.3":
            case "D6.4.1":
            case "D6.4.2":
            case "D6.4.3":
                StartCoroutine(Case6D62_call_to_X_ray());
                break;
        }
    }

    public void CheckSpecialEventCase7(string dialogue_id)
    {
        switch (dialogue_id)
        {
            case "D1.1":
                break;
        }
    }

    public void CheckSpecialEventCase9(string dialogue_id)
    {
        switch (dialogue_id)
        {
            case "D1.2":
                //StartCoroutine(Case9D12()); No es necesaria, en todo caso esta animacion deberia ir donde las basicas porque lo dice mientras habla (señalar a la pantalla)
                break;
            case "D4.1":
                StartCoroutine(Case9D41());
                break;
            case "D4.2":
                StartCoroutine(Case9D42());
                break;
            case "D4.3":
                StartCoroutine(Case9D43());
                break;
            case "D4.1.1":
            case "D4.1.3.3":
            case "D4.1.4.1":
                StartCoroutine(Case9D4141()); //Señala a la mesa y la enfermera da los forceps
                break;
            case "D6.4":
                StartCoroutine(Case9D64());
                break;
            case "D6.4.1":
                StartCoroutine(Case9D641());
                break;
            case "D6.4.3":
            case "D6.4.4":
                StartCoroutine(Case9D643());
                break;
            case "D6.1.3.1":
            case "D6.1.4.2":
                StartCoroutine(Case9D6142());
                break;
            case "D8.1":
                StartCoroutine(Case9D81());
                break;
            case "D8.1.1":
                StartCoroutine(Case9D811());
                break;
            case "D8.1.2":
            case "D8.1.3":
            case "D8.1.4":
                StartCoroutine(Case9D814());
                break;
            case "D9.1":
                StartCoroutine(Case9D91());
                break;

        }
    }

    #region CASE5_EVENTS
    IEnumerator Case5D01()
    {
        Debug.Log($"Suena el telefono y la circulating nurse lo coge, cuando acaba la llamada se gira para hablar con el equipo");
        playingAnimation = true;

        //Va al punto donde se encuentra el telefono
        //Ir a la mesa
        movementNurse = UI_Manager.Instance.dictionaryCharacteres["CirculatingNurse"].gameObject.GetComponent<SimpleWaypointMovement>();
        movementNurse.SetPathAndPlay(Case3Resources.Instance.waypointsToTable1, Case3Resources.Instance.tableLookPoint);
        while (movementNurse.canMove)
        {
            yield return null; //esperamos hasta que llegue a su destino, que sera cuando el canMove sea false
        }

        //Animacion de hablar
        //HABLA POR TELEFONO
        anim = "LookFor"; //TODO: Cambiar por la de count cuando este metida en el animator
        yield return PlaySimpleAnim(UI_Manager.Instance.dictionaryCharacteres["CirculatingNurse"].gameObject, anim);
        //Esperar unos segundos
        yield return new WaitForSeconds(2f);

        //Volverse al sitio
        Debug.Log($"Volvemos a nuestro sitio original");
        movementNurse = UI_Manager.Instance.dictionaryCharacteres["CirculatingNurse"].gameObject.GetComponent<SimpleWaypointMovement>();
        movementNurse.ResetPosition();
        while (movementNurse.canMove)
        {
            yield return null; //esperamos hasta que llegue a su destino, que sera cuando el canMove sea false
        }

        playingAnimation = false;
    }

    IEnumerator Case5D1613()
    {
        Debug.Log($"Final 1: todos se ponen a trabajar y después se enseñan al paciente, a la embarazada y el niño en sus habitaciones.");
        playingAnimation = true;

        yield return ShowPregnantWomanGood(true);

        playingAnimation = false;
    }

    IEnumerator Case5D1623()
    {
        Debug.Log($"Final 1: todos se ponen a trabajar y después se enseñan al paciente, a la embarazada y el niño en sus habitaciones.");
        playingAnimation = true;
        yield return ShowPregnantWomanGood(true);
        playingAnimation = false;
    }

    IEnumerator Case5D1633()
    {
        Debug.Log($"Final 1: todos se ponen a trabajar y después se enseñan al paciente, a la embarazada y el niño en sus habitaciones.");
        playingAnimation = true;
        yield return ShowPregnantWomanGood(true);
        playingAnimation = false;
    }

    IEnumerator Case5D1641()
    {
        Debug.Log($"Final 1: todos se ponen a trabajar y después se enseñan al paciente, a la embarazada y el niño en sus habitaciones.");
        playingAnimation = true;
        yield return ShowPregnantWomanGood(true);
        playingAnimation = false;
    }

    IEnumerator Case5D1812()
    {
        Debug.Log($"Final 2: Se enseña a la mujer que ha perdido al niño de forma dramática");
        playingAnimation = true;
        yield return ShowPregnantWomanGood(false);
        playingAnimation = false;
    }

    IEnumerator Case5D1822()
    {
        Debug.Log($"Final 2: Se enseña a la mujer que ha perdido al niño de forma dramática");
        playingAnimation = true;
        yield return ShowPregnantWomanGood(false);
        playingAnimation = false;
    }

    IEnumerator Case5D1832()
    {
        Debug.Log($"Final 2: Se enseña a la mujer que ha perdido al niño de forma dramática");
        playingAnimation = true;
        yield return ShowPregnantWomanGood(false);
        playingAnimation = false;
    }

    #endregion

    #region CASE6_EVENTS
    IEnumerator Case6D2_Count_towels()
    {
        Debug.Log($"Volver a contar los hisopos y toallas. (Instrumentalist )");
        playingAnimation = true;

        //IR A DONDE SEA:
        movementNurse = UI_Manager.Instance.dictionaryCharacteres["InstrumentalistNurse"].gameObject.GetComponent<SimpleWaypointMovement>();
        movementNurse.SetPathAndPlay(Case3Resources.Instance.waypointsToTable1, Case3Resources.Instance.tableLookPoint);
        while (movementNurse.canMove)
        {
            yield return null; //esperamos hasta que llegue a su destino, que sera cuando el canMove sea false
        }

        //ANIMACION DE CONTAR TOALLAS
        anim = "LookFor"; //TODO: Cambiar por la de count cuando este metida en el animator
        yield return PlaySimpleAnim(UI_Manager.Instance.dictionaryCharacteres["InstrumentalistNurse"].gameObject, anim);


        //VOLVER DEL SITIO
        //Volverse al sitio
        Debug.Log($"Volvemos a nuestro sitio original");
        movementNurse = UI_Manager.Instance.dictionaryCharacteres["InstrumentalistNurse"].gameObject.GetComponent<SimpleWaypointMovement>();
        movementNurse.ResetPosition();
        while (movementNurse.canMove)
        {
            yield return null; //esperamos hasta que llegue a su destino, que sera cuando el canMove sea false
        }


        playingAnimation = false;
    }

    IEnumerator Case6D4_search_in_floor()
    {
        Debug.Log($"Mirar por el suelo y debajo de los equipos. (todos)");
        playingAnimation = true;

        yield return PlayBroadcastAnim("LookFor"); //TODO: Cambiar por la animacion correcta

        playingAnimation = false;
    }

    IEnumerator Case6D62_call_to_X_ray()
    {
        Debug.Log($"Anaesthesia assistant llama a la circulating nurse para hacer el rayo x. (¿Por telefono?)");
        playingAnimation = true;


        //Va al punto donde se encuentra el telefono
        //Ir a la mesa
        movementNurse = UI_Manager.Instance.dictionaryCharacteres["AssistantSurgeon"].gameObject.GetComponent<SimpleWaypointMovement>();
        movementNurse.SetPathAndPlay(Case3Resources.Instance.waypointsToTable1, Case3Resources.Instance.tableLookPoint);
        while (movementNurse.canMove)
        {
            yield return null; //esperamos hasta que llegue a su destino, que sera cuando el canMove sea false
        }

        //Animacion de hablar

        //HABLA POR TELEFONO
        anim = "LookFor"; //TODO: Cambiar por la de count cuando este metida en el animator
        yield return PlaySimpleAnim(UI_Manager.Instance.dictionaryCharacteres["AssistantSurgeon"].gameObject, anim);

        //Esperar unos segundos 
        yield return new WaitForSeconds(2f);

        //Volverse al sitio
        Debug.Log($"Volvemos a nuestro sitio original");
        movementNurse = UI_Manager.Instance.dictionaryCharacteres["AssistantSurgeon"].gameObject.GetComponent<SimpleWaypointMovement>();
        movementNurse.ResetPosition();
        while (movementNurse.canMove)
        {
            yield return null; //esperamos hasta que llegue a su destino, que sera cuando el canMove sea false
        }


        playingAnimation = false;
    }

    #endregion

    #region CASE9_EVENTS


    IEnumerator Case9D41()
    {
        Debug.Log($"Endoscopy nurse da inyector");
        playingAnimation = true;

        yield return GoToTableAndGiveTool("Inyector");

        playingAnimation = false;
    }

    IEnumerator Case9D42()
    {
        Debug.Log($"Endoscopy nurse da inyección submucosa");
        playingAnimation = true;
        yield return GoToTableAndGiveTool("Forceps"); //TODO: DEBE SER LA INYECCION SUBMUCOSA
        playingAnimation = false;
    }

    IEnumerator Case9D43()
    {
        Debug.Log($"Endoscopy nurse da lazo polipectomico/asa de polipectomia??");
        playingAnimation = true;
        yield return GoToTableAndGiveTool("Lazo");
        playingAnimation = false;
    }

    IEnumerator GoToTableAndGiveTool(string tool, string character = "EndoscopyNurse")
    {
        //Ir a la mesa
        //Va al punto donde se encuentra el telefono
        //Ir a la mesa
        movementNurse = UI_Manager.Instance.dictionaryCharacteres[character].gameObject.GetComponent<SimpleWaypointMovement>();
        movementNurse.SetPathAndPlay(Case3Resources.Instance.waypointsToTable1, Case3Resources.Instance.tableLookPoint);
        while (movementNurse.canMove)
        {
            yield return null; //esperamos hasta que llegue a su destino, que sera cuando el canMove sea false
        }

        //Animacion de buscar
        anim = "LookFor"; 
        yield return PlaySimpleAnim(UI_Manager.Instance.dictionaryCharacteres[character].gameObject, anim);

        // coger el inyector
        //poner el prop en la mano de la enfermera
        prop = GetProp(tool); //creo que es forceps
        position = UI_Manager.Instance.dictionaryCharacteres[character].gameObject.GetComponent<HandPosition>().handPos;
        SetProp(position, prop);
        nurseTool = tool;


        //Volverse al sitio
        Debug.Log($"Volvemos a nuestro sitio original");
        movementNurse = UI_Manager.Instance.dictionaryCharacteres[character].gameObject.GetComponent<SimpleWaypointMovement>();
        movementNurse.ResetPosition();
        while (movementNurse.canMove)
        {
            yield return null; //esperamos hasta que llegue a su destino, que sera cuando el canMove sea false
        }

        //le das el inyector al endoescopista
        anim = "Give";
        yield return PlaySimpleAnim(UI_Manager.Instance.dictionaryCharacteres[character].gameObject, anim, tool);



    }

    IEnumerator Case9D4141()
    {
        Debug.Log($"Señala a la mesa y la enfermera da los forceps");
        playingAnimation = true;
        yield return GoToTableAndGiveTool("Forceps");
        playingAnimation = false;
    }
    IEnumerator Case9D64()
    {
        Debug.Log($"La enfermera va a la mesa y coge una herramienta que no es un coag grasper");
        playingAnimation = true;
        yield return GoToTableAndGiveTool("HerramientaErronea");
        playingAnimation = false;
    }

    IEnumerator Case9D641()
    {
        Debug.Log($"Le tiras la herramienta a la cabeza");
        playingAnimation = true;
        anim = "Throw";
        yield return PlaySimpleAnim(UI_Manager.Instance.dictionaryCharacteres["Endoscopist1"].gameObject, anim);
        currentTool = "";
        playingAnimation = false;
    }

    IEnumerator Case9D643()
    {
        Debug.Log($"Le devuelve la herramienta");
        playingAnimation = true;

        anim = "Give";
        yield return PlaySimpleAnim(UI_Manager.Instance.dictionaryCharacteres["Endoscopist1"].gameObject, anim, "HerramientaErronea", "EndoscopyNurse");

        playingAnimation = false;
    }

    IEnumerator Case9D6142()
    {
        Debug.Log($"La endoscopist 2 se mueve a la mesa de herramientas junto a la endoscopy nurse y te dan la herramienta "); //TODO: PONER coag grasper EN LUGAR DEL Forceps
        playingAnimation = true;


        //TANTO LA ENFERMERA COMO EL ENDOSCOPIST 2 VAN A LA MESA

 

        //poner en la mano de la enfermera el coaggrasper

        //volver cada una a su sitio

        movementNurse = UI_Manager.Instance.dictionaryCharacteres["EndoscopyNurse"].gameObject.GetComponent<SimpleWaypointMovement>();
        movementNurse.SetPathAndPlay(Case3Resources.Instance.waypointsToTable1, Case3Resources.Instance.tableLookPoint);

        movementNurse = UI_Manager.Instance.dictionaryCharacteres["Endoscopist2"].gameObject.GetComponent<SimpleWaypointMovement>();
        movementNurse.SetPathAndPlay(Case3Resources.Instance.waypointsToTable2, Case3Resources.Instance.tableLookPoint);
        while (movementNurse.canMove)
        {
            yield return null; //esperamos hasta que llegue a su destino, que sera cuando el canMove sea false
        }

        //Animacion de buscar
        anim = "LookFor";
        yield return PlaySimpleAnim(UI_Manager.Instance.dictionaryCharacteres["Endoscopist2"].gameObject, anim);

        //Animacion de buscar
        anim = "LookFor";
        yield return PlaySimpleAnim(UI_Manager.Instance.dictionaryCharacteres["EndoscopyNurse"].gameObject, anim);

        // coger la herramienta (enfermera)
        //poner el prop en la mano de la enfermera
        prop = GetProp("Forceps"); //creo que es forceps
        position = UI_Manager.Instance.dictionaryCharacteres["EndoscopyNurse"].gameObject.GetComponent<HandPosition>().handPos;
        SetProp(position, prop);
        nurseTool = "Forceps";

        //Volverse al sitio
        Debug.Log($"Volvemos a nuestro sitio original");
        movementNurse = UI_Manager.Instance.dictionaryCharacteres["EndoscopyNurse"].gameObject.GetComponent<SimpleWaypointMovement>();
        movementNurse.ResetPosition();

        //Volverse al sitio
        Debug.Log($"Volvemos a nuestro sitio original");
        movementNurse = UI_Manager.Instance.dictionaryCharacteres["Endoscopist2"].gameObject.GetComponent<SimpleWaypointMovement>();
        movementNurse.ResetPosition();
        while (movementNurse.canMove)
        {
            yield return null; //esperamos hasta que llegue a su destino, que sera cuando el canMove sea false
        }

        //le das el inyector al endoescopista
        anim = "Give";
        yield return PlaySimpleAnim(UI_Manager.Instance.dictionaryCharacteres["EndoscopyNurse"].gameObject, anim, "Forceps");


        playingAnimation = false;
    }

    IEnumerator Case9D81()
    {
        Debug.Log($"Entra el secretario");
        playingAnimation = true;

        UI_Manager.Instance.generalArrow.SetActive(true);
        UI_Manager.Instance.generalArrow.GetComponent<LookTarget>().target = UI_Manager.Instance.dictionaryCharacteres["Secretary"].gameObject.transform;
        SimpleWaypointMovement movement = Case3Resources.Instance.secretary.GetComponent<SimpleWaypointMovement>();
        //Case3Resources.Instance.doorAnim.Play();
        movement.canMove = true;
        while (movement.canMove)
        {
            yield return null; //esperamos hasta que llegue a su destino, que sera cuando el canMove sea false
        }

        playingAnimation = false;
    }

    IEnumerator Case9D811()
    {
        Debug.Log($"Entran estudiantes y los que están dentro se mueven un poco, el secretario se va despues");
        playingAnimation = true;

        //TODO: ENTRAN LOS NUEVOS ESTUDIANTES ¿? ES BUENA IDEA HACERLO?


        //SE VA EL SECRETARIO

        UI_Manager.Instance.generalArrow.SetActive(true);
        UI_Manager.Instance.generalArrow.GetComponent<LookTarget>().target = UI_Manager.Instance.dictionaryCharacteres["Secretary"].gameObject.transform;
        SimpleWaypointMovement movement = Case3Resources.Instance.secretary.GetComponent<SimpleWaypointMovement>();
        //Case3Resources.Instance.doorAnim.Play();
        movement.ResetPosition();
        while (movement.canMove)
        {
            yield return null; //esperamos hasta que llegue a su destino, que sera cuando el canMove sea false
        }

        playingAnimation = false;
    }

    IEnumerator Case9D814()
    {
        Debug.Log($"El secretario se va");
        playingAnimation = true;

        UI_Manager.Instance.generalArrow.SetActive(true);
        UI_Manager.Instance.generalArrow.GetComponent<LookTarget>().target = UI_Manager.Instance.dictionaryCharacteres["Secretary"].gameObject.transform;
        SimpleWaypointMovement movement = Case3Resources.Instance.secretary.GetComponent<SimpleWaypointMovement>();
        //Case3Resources.Instance.doorAnim.Play();
        movement.ResetPosition();
        while (movement.canMove)
        {
            yield return null; //esperamos hasta que llegue a su destino, que sera cuando el canMove sea false
        }


        playingAnimation = false;
    }

    IEnumerator Case9D91()
    {
        Debug.Log($"Devuelve las herramientas a la enfermera"); //¿es el forceps?
        playingAnimation = true;

        anim = "Give";
        yield return PlaySimpleAnim(UI_Manager.Instance.dictionaryCharacteres["Endoscopist1"].gameObject, anim, "Forceps", "EndoscopyNurse");

        playingAnimation = false;
    }

    #endregion

    #region CASE3_EVENTS
    IEnumerator CaseD11()
    {
        Debug.Log($"Vamos a hacer el caso D1.1: ponemos el endoscopio en la mano del endoescopista");
        playingAnimation = true;
        prop = GetProp("Endoscope");
        currentTool = "Endoscope";
        position = UI_Manager.Instance.dictionaryCharacteres["Endoscopist1"].gameObject.GetComponent<HandPosition>().handPos;
        SetProp(position, prop);// lo ponemos en tu mano
        yield return null;
        playingAnimation = false;
    }

    IEnumerator CaseD112()
    {
        //La enfermera se pone la mascarilla
        //Case3Resources.Instance.animMascarilla.SetActive(true);//activar la mascarilla de la animacion
        //play anim
        playingAnimation = true;
        UI_Manager.Instance.dictionaryCharacteres["EndoscopyNurse"].SetBool("maskAnim", true);

        //Esperar hasta que acabe la animacion -- dura 4.5
        yield return new WaitForSeconds(5.5f);

        playingAnimation = false;

        //Case3Resources.Instance.animMascarilla.SetActive(false); //desactivamos la mascarilla de la animacion
        //Case3Resources.Instance.mask.SetActive(true);//activar al final //Esto ahora lo hace desde el aimator al acabar la animacion
    }

    IEnumerator CaseD31()
    {
        Debug.Log($"Vamos a hacer el caso D3.1: La enfermera sale llorando");
        playingAnimation = true;
        movementNurse = Case3Resources.Instance.nurse.GetComponent<SimpleWaypointMovement>();
        //Case3Resources.Instance.doorAnim.Play();
        movementNurse.SetPathAndPlay(Case3Resources.Instance.waypointsExit, Case3Resources.Instance.endoscopist1LookPoint);
        while (movementNurse.canMove)
        {
            yield return null; //esperamos hasta que llegue a su destino, que sera cuando el canMove sea false
        }
        playingAnimation = false;
    }

    IEnumerator CaseD32()
    {
        Debug.Log($"Vamos a hacer el caso D3.2: La nueva enfermera entra");
        playingAnimation = true;
        UpdateNurseName();
        movementNurse = Case3Resources.Instance.nurse.GetComponent<SimpleWaypointMovement>();
        //Case3Resources.Instance.doorAnim.Play();
        movementNurse.SetPathAndPlay(Case3Resources.Instance.waypointsEnter, Case3Resources.Instance.endoscopist1LookPoint);
        while (movementNurse.canMove)
        {
            yield return null; //esperamos hasta que llegue a su destino, que sera cuando el canMove sea false
        }
        playingAnimation = false;
    }

    IEnumerator CaseD43()
    {
        Debug.Log($"Pregunta si es este el inyector (va a la mesa y lo coge antes)");
        playingAnimation = true;

        yield return GoToTableAndTakeObject("Inyector");

        //Se lo da al endoescopista1
        //Animacion de dar
        anim = "Give";
        yield return PlaySimpleAnim(UI_Manager.Instance.dictionaryCharacteres["EndoscopyNurse"].gameObject, anim, "Inyector", "Endoscopist1");

        playingAnimation = false;
    }

    IEnumerator CaseD51() 
    {
        Debug.Log($"Vamos a hacer el caso D5.1: La enfermera sale llorando");
        playingAnimation = true;
        movementNurse = Case3Resources.Instance.nurse.GetComponent<SimpleWaypointMovement>();
        //Case3Resources.Instance.doorAnim.Play();
        movementNurse.SetPathAndPlay(Case3Resources.Instance.waypointsExit, Case3Resources.Instance.endoscopist1LookPoint);
        while (movementNurse.canMove)
        {
            yield return null; //esperamos hasta que llegue a su destino, que sera cuando el canMove sea false
        }
        playingAnimation = false;
    }

    IEnumerator CaseD52()
    {
        Debug.Log($"Vamos a hacer el caso D5.2: La nueva enfermera entra");
        playingAnimation = true;
        UpdateNurseName();
        movementNurse = Case3Resources.Instance.nurse.GetComponent<SimpleWaypointMovement>();
        //Case3Resources.Instance.doorAnim.Play();
        movementNurse.SetPathAndPlay(Case3Resources.Instance.waypointsEnter, Case3Resources.Instance.endoscopist1LookPoint);
        while (movementNurse.canMove)
        {
            yield return null; //esperamos hasta que llegue a su destino, que sera cuando el canMove sea false
        }
        playingAnimation = false;
    }

    IEnumerator CaseD62()
    {
        Debug.Log($"Vamos a hacer el caso D6.2: La enfermera te da el lazo, extraes el endoscopio y se lo das a la enfermera y la enfermera lo deja en la mesa");
        //la enfermera te da el lazo, extraes el endoscopio y se lo das a la enfermera y la enfermera lo deja en la mesa
        playingAnimation = true;

        //La enfermera va a la mesa y busca el lazo, lo ponemos en la mano y vuelve
        yield return GoToTableAndTakeObject("Lazo");

        Debug.Log($"Hace la animacion de dartelo");
        //Animacion de darme el lazo
        anim = "Give"; // en las de dar podemos hacer que mire a un target
        yield return PlaySimpleAnim(UI_Manager.Instance.dictionaryCharacteres["EndoscopyNurse"].gameObject, anim, "Lazo", "Endoscopist1");

        //TODO: extraigo endoscopio  ¿animacion?
        //Debug.Log($"Extraemos el endoscopio");
        //Animacion de sacar el endoscopio //TODO
        anim = "EndoscopeAnim"; // en las de dar podemos ahcer que mire a un target
        yield return PlaySimpleAnim(UI_Manager.Instance.dictionaryCharacteres["Endoscopist1"].gameObject, anim);

        //Debug.Log($"Lo ponemos en tu mano");
        //Lo ponemos en tu mano
        prop = GetProp("Endoscope");
        position = UI_Manager.Instance.dictionaryCharacteres["Endoscopist1"].gameObject.GetComponent<HandPosition>().handPos;
        SetProp(position, prop);
        //Si ya tiene algo lo quitamos y lo ponemos en la mesa
        if (currentTool != null && currentTool != "Herramienta" && currentTool != "")
        {
            Case3Resources.Instance.PutInExtraTable(GetProp(currentTool));
        }
        currentTool = "Endoscope";

        //Debug.Log($"Animacion de darselo a la enfermera");
        //Se los das a la enfermera
        anim = "Give";
        yield return PlaySimpleAnim(UI_Manager.Instance.dictionaryCharacteres["Endoscopist1"].gameObject, anim, "Endoscope", "EndoscopyNurse");

        //Debug.Log($"La enfermera va a la mesa");
        yield return GoToTableAndLeaveObject("Endoscope");

        playingAnimation = false;
    }

    IEnumerator CaseD64()
    {
        Debug.Log($"Vamos a hacer el caso D6.4: La enfermera Rebusca en la mesa y te da una herramienta erronea");
        //rebusca en la mesa y te da una herramienta erronea
        playingAnimation = true;

        //antes de esto le devolvemos la herramienta que estamos usando --> esto ya no hace falta con la mesa extra
        //animacion de dar herramienta
        /*
        anim = "Give";
        yield return PlaySimpleAnim(UI_Manager.Instance.dictionaryCharacteres["Endoscopist1"].gameObject, anim, currentTool, "EndoscopyNurse");
        */

        yield return GoToTableAndTakeObject("HerramientaErronea");

        Debug.Log("La enfermera te da el objeto");
        //Animacion de dar
        anim = "Give";
        yield return PlaySimpleAnim(UI_Manager.Instance.dictionaryCharacteres["EndoscopyNurse"].gameObject, anim, "HerramientaErronea", "Endoscopist1");
        // esto ya lo hace el give: nurseTool = "";

        playingAnimation = false;
    }

    IEnumerator CaseD611()
    {
        Debug.Log($"Vamos a hacer el caso D6.1.1: lanzas la herramienta de vuelta");

        Debug.Log($"Animacion de lanzar");
        //lanzas la herramienta de vuelta
        playingAnimation = true;
        anim = "Throw";
        yield return PlaySimpleAnim(UI_Manager.Instance.dictionaryCharacteres["Endoscopist1"].gameObject, anim);
        currentTool = "";


        Debug.Log($"movimiento de la herramienta");
        //TODO: Movimiento de la herramienta (lo hacemos con rigid body¿?)
        /*
        //eliminar herramienta de la mano y lanzarla contra la enfermera //bastaria con ponerle un follow point que se vaya moviendo //To test
        float interpolationRatio = 0;
        float speed = 2f;
        Vector3 origin = prop.transform.position;
        Vector3 target = UI_Manager.Instance.dictionaryCharacteres["EndoscopyNurse"].gameObject.transform.position;
        //prop.GetComponent<FollowPoint>().target = null; //de esta forma nos hara caso a nosotros y no al follow point

        //fake movement
        while (interpolationRatio < 1)
        {
            interpolationRatio += Time.deltaTime * speed;
            prop.GetComponent<FollowPoint>().target = Vector3.Lerp(origin, target, interpolationRatio);
            yield return null;
        }
        */
        //DeleteProp(prop); //temporal
        playingAnimation = false;
    }

    IEnumerator CaseD613()
    {
        Debug.Log($"Vamos a hacer el caso D6.1.3: le devuelves la herramienta");
        //le devuelves la herramienta
        playingAnimation = true;

        Debug.Log($"Haces la animacion de darsela");
        anim = "Give";
        yield return PlaySimpleAnim(UI_Manager.Instance.dictionaryCharacteres["Endoscopist1"].gameObject, anim, "HerramientaErronea", "EndoscopyNurse");

        yield return GoToTableAndLeaveObject("HerramientaErronea");

        playingAnimation = false;
    }

    IEnumerator CaseD643()
    {
        Debug.Log($"Vamos a hacer el caso D6.4.3: el endoescopista 2 ayuda a la enfermera a buscar el forceps en la mesa, ambas enfermera tienen que ir a la mesa");
        // el endoescopista 2 ayuda a la enfermera a buscar el forceps en la mesa, ambas enfermera tienen que ir a la mesa
        playingAnimation = true;
        //Ir a la mesa ambos

        Debug.Log($"La enfermera va a la mesa");
        //ir a la mesa enfermera
        movementNurse = UI_Manager.Instance.dictionaryCharacteres["EndoscopyNurse"].gameObject.GetComponent<SimpleWaypointMovement>();
        movementNurse.SetPathAndPlay(Case3Resources.Instance.waypointsToTable1, Case3Resources.Instance.tableLookPoint);
        while (movementNurse.canMove) //este while igual nos lo podemos machacar en este contexto, y que se quede esperando en el siguiente
        {
            yield return null; //esperamos hasta que llegue a su destino, que sera cuando el canMove sea false
        }
        Debug.Log($"El Endoscopist2 va a la mesa");
        //ir a la mesa endoescopista 2
        endoscopist2Mov = UI_Manager.Instance.dictionaryCharacteres["Endoscopist2"].gameObject.GetComponent<SimpleWaypointMovement>();
        endoscopist2Mov.SetPathAndPlay(Case3Resources.Instance.waypointsToTable2, Case3Resources.Instance.tableLookPoint);
        while (endoscopist2Mov.canMove)
        {
            yield return null; //esperamos hasta que llegue a su destino, que sera cuando el canMove sea false
        }

        Debug.Log($"La enfermera hace la animacion de buscar");
        //Anim de buscar:
        anim = "LookFor";
        yield return PlaySimpleAnim(UI_Manager.Instance.dictionaryCharacteres["EndoscopyNurse"].gameObject, anim);

        Debug.Log($"El endoescopista 2 hace la animacion de buscar");
        yield return PlaySimpleAnim(UI_Manager.Instance.dictionaryCharacteres["Endoscopist2"].gameObject, anim);

        Debug.Log($"Ponemos la herramienta en la mano de la enfermera");
        //poner el prop en la mano de la enfermera
        prop = GetProp("Forceps"); //creo que es forceps
        position = UI_Manager.Instance.dictionaryCharacteres["EndoscopyNurse"].gameObject.GetComponent<HandPosition>().handPos;
        SetProp(position, prop);
        nurseTool = "Forceps";

        Debug.Log($"La enfermera se vuelve a su sitio");
        //volver a sus sitios
        movementNurse = UI_Manager.Instance.dictionaryCharacteres["EndoscopyNurse"].gameObject.GetComponent<SimpleWaypointMovement>();
        movementNurse.ResetPosition();
        while (movementNurse.canMove)
        {
            yield return null; //esperamos hasta que llegue a su destino, que sera cuando el canMove sea false
        }
        Debug.Log($"El Endoscopist2 se vuelve a su sitio");
        endoscopist2Mov = UI_Manager.Instance.dictionaryCharacteres["Endoscopist2"].gameObject.GetComponent<SimpleWaypointMovement>();
        endoscopist2Mov.ResetPosition();
        while (endoscopist2Mov.canMove)
        {
            yield return null; //esperamos hasta que llegue a su destino, que sera cuando el canMove sea false
        }

        //No te da la herramienta aqui sino en el siguiente (D71)
        /*Debug.Log($"La enfermera te da la herramienta");
        //Te da la herramienta
        anim = "Give";
        yield return PlaySimpleAnim(UI_Manager.Instance.dictionaryCharacteres["EndoscopyNurse"].gameObject, anim, "Herramienta", "Endoscopist1");
        */
        playingAnimation = false;
    }

    IEnumerator CaseD640()
    {
        playingAnimation = true;
        //El end2 se gira hacia la enfermera
        yield return TurnsTo(UI_Manager.Instance.dictionaryCharacteres["Endoscopist2"].gameObject, UI_Manager.Instance.dictionaryCharacteres["EndoscopyNurse"].gameObject.transform.position);
        playingAnimation = false;
    }
    IEnumerator CaseD65()
    {
        //el endoscopista 2 vuelve a su pos inicial
        playingAnimation = true;

        yield return TurnsTo(UI_Manager.Instance.dictionaryCharacteres["Endoscopist2"].gameObject, UI_Manager.Instance.dictionaryCharacteres["Endoscopist1"].gameObject.transform.position);
        playingAnimation = false;
    }

    IEnumerator CaseD71()
    {
        //la enfermera te pasa el forceps
        playingAnimation = true;
        anim = "Give";
        yield return PlaySimpleAnim(UI_Manager.Instance.dictionaryCharacteres["EndoscopyNurse"].gameObject, anim, "Forceps", "Endoscopist1");

        playingAnimation = false;
    }

    IEnumerator CaseD81()
    {
        //la enfermera busca nerviosa la herramienta y se le cae al suelo
        playingAnimation = true;

        //ir a la mesa enfermera
        movementNurse = UI_Manager.Instance.dictionaryCharacteres["EndoscopyNurse"].gameObject.GetComponent<SimpleWaypointMovement>();
        movementNurse.SetPathAndPlay(Case3Resources.Instance.waypointsToTable1, Case3Resources.Instance.tableLookPoint);
        while (movementNurse.canMove) //este while igual nos lo podemos machacar en este contexto, y que se quede esperando en el siguiente
        {
            yield return null; //esperamos hasta que llegue a su destino, que sera cuando el canMove sea false
        }

        //Anim de buscar:
        anim = "LookFor";
        yield return PlaySimpleAnim(UI_Manager.Instance.dictionaryCharacteres["EndoscopyNurse"].gameObject, anim);

        //Se le cae (lo seteamos directamente y pa alante)
        prop = GetProp("HerramientaErronea");
        position = Case3Resources.Instance.floorPoint.transform;
        SetProp(position, prop);

        playingAnimation = false;
    }

    IEnumerator CaseD82()
    {
        //volver a su posicion (enfermera)
        playingAnimation = true;

        //volver a su sitio
        movementNurse = UI_Manager.Instance.dictionaryCharacteres["EndoscopyNurse"].gameObject.GetComponent<SimpleWaypointMovement>();
        movementNurse.ResetPosition();
        while (movementNurse.canMove)
        {
            yield return null; //esperamos hasta que llegue a su destino, que sera cuando el canMove sea false
        }

        playingAnimation = false;
    }
    IEnumerator CaseD111()
    {
        UI_Manager.Instance.generalArrow.SetActive(true);
        UI_Manager.Instance.generalArrow.GetComponent<LookTarget>().target = UI_Manager.Instance.dictionaryCharacteres["Secretary"].gameObject.transform;
        playingAnimation = true;
        SimpleWaypointMovement movement = Case3Resources.Instance.secretary.GetComponent<SimpleWaypointMovement>();
        //Case3Resources.Instance.doorAnim.Play();
        movement.canMove = true;
        while (movement.canMove)
        {
            yield return null; //esperamos hasta que llegue a su destino, que sera cuando el canMove sea false
        }
        playingAnimation = false;
    }

    IEnumerator CaseD101()
    {
        playingAnimation = true;
        isExtraNurse = true;
        UpdateNurseName();
        SimpleWaypointMovement movement = UI_Manager.Instance.dictionaryCharacteres["EndoscopyNurseExtra"].gameObject.GetComponent<SimpleWaypointMovement>();
        //Case3Resources.Instance.doorAnim.Play();
        movement.SetPathAndPlay(Case3Resources.Instance.waypointsEnterExtra, Case3Resources.Instance.endoscopist1LookPoint);
        while (movement.canMove)
        {
            yield return null; //esperamos hasta que llegue a su destino, que sera cuando el canMove sea false
        }
        playingAnimation = false;
    }

    #endregion

    #region UTILS

    IEnumerator ShowPregnantWomanGood(bool isGoodFinal)
    {
        if (isGoodFinal)
        {
            //TODO: ACTIVAR RENDER CORRESPONDIENTE
        }
        else
        {
            //TODO: ACTIVAR RENDER CORRESPONDIENTE
        }

        yield return new WaitForSeconds(5);

        //desactivamos el render

        if (isGoodFinal)
        {
            //TODO: DESACTIVAMOS EL RENDER CORRESPONDIENTE
        }
        else
        {
            //TODO: DESACTIVAMOS EL RENDER CORRESPONDIENTE
        }
    }
    IEnumerator GoToTableAndTakeObject(string tableObject)
    {
        Debug.Log("La enfermera va a la mesa");
        //Ir a la mesa
        movementNurse = UI_Manager.Instance.dictionaryCharacteres["EndoscopyNurse"].gameObject.GetComponent<SimpleWaypointMovement>();
        movementNurse.SetPathAndPlay(Case3Resources.Instance.waypointsToTable1, Case3Resources.Instance.tableLookPoint);
        while (movementNurse.canMove)
        {
            yield return null; //esperamos hasta que llegue a su destino, que sera cuando el canMove sea false
        }

        Debug.Log("La enfermera hace la animacion de buscar");
        //Animacion de buscar
        anim = "LookFor";
        yield return PlaySimpleAnim(UI_Manager.Instance.dictionaryCharacteres["EndoscopyNurse"].gameObject, anim);

        //si tenemos una herramienta en la mano la dejamos en la mesa antes de coger la nueva
        if (nurseTool != null && nurseTool != "")
        {
            prop = GetProp(nurseTool);
            GameObject posAux = new GameObject();
            posAux.transform.position = Case3Resources.Instance.positionsDictionary[prop.name];
            //Case3Resources.Instance.tablePoint.transform;
            SetProp(posAux.transform, prop);
            nurseTool = "";
        }

        Debug.Log("Ponemos el objeto en la mano de la enfermera");
        //Aparecer el objeto en la mano
        prop = GetProp(tableObject);
        nurseTool = tableObject;
        position = UI_Manager.Instance.dictionaryCharacteres["EndoscopyNurse"].gameObject.GetComponent<HandPosition>().handPos;
        SetProp(position, prop);// lo ponemos en su mano

        Debug.Log("La enfermera se vuelve a su sitio");
        //volver a su sitio
        movementNurse = UI_Manager.Instance.dictionaryCharacteres["EndoscopyNurse"].gameObject.GetComponent<SimpleWaypointMovement>();
        movementNurse.ResetPosition();
        while (movementNurse.canMove)
        {
            yield return null; //esperamos hasta que llegue a su destino, que sera cuando el canMove sea false
        }
    }

    IEnumerator GoToTableAndLeaveObject(string tableObject)
    {
        Debug.Log("La enfermera va a la mesa");
        //Ir a la mesa
        movementNurse = UI_Manager.Instance.dictionaryCharacteres["EndoscopyNurse"].gameObject.GetComponent<SimpleWaypointMovement>();
        movementNurse.SetPathAndPlay(Case3Resources.Instance.waypointsToTable1, Case3Resources.Instance.tableLookPoint);
        while (movementNurse.canMove)
        {
            yield return null; //esperamos hasta que llegue a su destino, que sera cuando el canMove sea false
        }

        Debug.Log("La enfermera hace la animacion de buscar");
        //Animacion de buscar
        anim = "LookFor";
        yield return PlaySimpleAnim(UI_Manager.Instance.dictionaryCharacteres["EndoscopyNurse"].gameObject, anim);

        Debug.Log($"Ponemos la herramienta en la mesa");
        prop = GetProp(tableObject);
        GameObject posAux = new GameObject();
        posAux.transform.position = Case3Resources.Instance.positionsDictionary[prop.name];
        //Case3Resources.Instance.tablePoint.transform;
        SetProp(posAux.transform, prop);
        nurseTool = "";

        Debug.Log("La enfermera se vuelve a su sitio");
        //volver a su sitio
        movementNurse = UI_Manager.Instance.dictionaryCharacteres["EndoscopyNurse"].gameObject.GetComponent<SimpleWaypointMovement>();
        movementNurse.ResetPosition();
        while (movementNurse.canMove)
        {
            yield return null; //esperamos hasta que llegue a su destino, que sera cuando el canMove sea false
        }
    }

    IEnumerator TurnsTo(GameObject characer, Vector3 targetPosition) //para volver a la posicion original basta con indicar que se giren hacia en endoescopista 1
    {
        //activar aniamcion de girar (hay que comprobar hacia donde gira y en funcion de eso poner derecha o izquierda)
        Animator animator = characer.GetComponent<Animator>();
        float turnAnimDuration = 0f;
        float rotationSpeed = 2f;
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

        //Debug.Log($"Almacenamos la duracion de la animación actual de girarase = {turnAnimDuration}");

        //Opcion 1 (con animacion haciendo cosas raras)
        //yield return new WaitForSeconds(turnAnimDuration);
        //characer.transform.LookAt(targetPosition);

        float duration = 2f;
        //Opcion 2 (sin animacion)
        //Math.Abs(Vector3.Distance(characer.transform.forward, targetPosition - characer.transform.position)) > 0.2f
        while (duration > 0)//characer.transform.rotation != Quaternion.LookRotation(targetPosition - characer.transform.position)
        {
            duration -= Time.deltaTime;
            Debug.Log($"Seguimos girando hasta que encontremos al player, distancia: {Math.Abs(Vector3.Distance(characer.transform.forward, targetPosition - characer.transform.position))}");
            characer.transform.rotation = Quaternion.Lerp(characer.transform.rotation, Quaternion.LookRotation(targetPosition - characer.transform.position), Time.deltaTime * rotationSpeed);
            yield return null;
        }

    }

    IEnumerator PlayBroadcastAnim(string animName)
    {
        playingAnimation = true;
        float animDuration = 0;
        bool firstTime = true;
        foreach (var item in UI_Manager.Instance.dictionaryCharacteres)
        {
            item.Value.SetBool(animName, true);
            if (firstTime)
            {
                animDuration = GetDuration(animName, item.Value);
                firstTime = false;
            }
        }
        
        yield return new WaitForSeconds(animDuration + 2f);
    }

    IEnumerator PlaySimpleAnim(GameObject character, string animName, string propName = "Herramienta", string target = "Endoscopist1")
    {
        //Parametros de entrada: quien hace la anumacion, nombre de la animacion, nombre de la herramienta, hacia quien hace la animacion
        playingAnimation = true;
        //IMPORTANTE: La animacion y la transicion deben de tener el mismo nombre para que funcione bien
        //Si se queda en bucle la animacion revisar si se ha añadido el animManager y el false a la animacion dentro del mismo 
        Animator animator = character.GetComponent<Animator>();
        animator.SetBool(animName, true);
        float animDuration = GetDuration(animName, animator);

        if (animName == "Give")
        {
            //a mitad de la animacion hace desaparecer el objeto de la mano
            yield return new WaitForSeconds(animDuration * 0.75f);

            prop = GetProp(propName);
            position = UI_Manager.Instance.dictionaryCharacteres[target].gameObject.GetComponent<HandPosition>().handPos;

            // Si ya tiene algo en la mano ponerlo en la mesa
            if (target == "Endoscopist1" && currentTool != null && currentTool != "Herramienta" && currentTool != "")
            {
                Case3Resources.Instance.PutInExtraTable(GetProp(currentTool));
            }

            //Aqui podriamos comprobar si la enfermera tiene algo en la mano, pero en teoria nunca deberia tenerlo llegados a este punto

            //Ponemos la herramienta en la mano
            SetProp(position, prop);
            // Esrto solo se llama si es el endoescopista: currentTool = propName;

            yield return new WaitForSeconds(animDuration * 0.25f + 0.4f);

            if (target == "Endoscopist1")
            {
                currentTool = propName;
                nurseTool = "";
            }
            else if (target == "EndoscopyNurse")
            {
                nurseTool = propName;
                currentTool = "";
            }
        }
        else
        {
            yield return new WaitForSeconds(animDuration + 0.4f); //esperamos un poco de tiempo adicional 
        }
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
        GameObject aux = prop; //default prop
        //Consultar un diccionario de herramientas en el case 3 resources
        if (Case3Resources.Instance.toolsDictionary.ContainsKey(propName))
        {
            aux = Case3Resources.Instance.toolsDictionary[propName];
        }

        return aux;
    }

    public IEnumerator ExitRoom()
    {
        //Todos abandonan la habitación
        //Case3Resources.Instance.doorAnim.Play(); //revisar esto

        //anestesiologo
        SimpleWaypointMovement aux = Case3Resources.Instance.anaesthesiologist.GetComponent<SimpleWaypointMovement>();
        aux.SetPathAndPlay(Case3Resources.Instance.waypointsExit, Case3Resources.Instance.endoscopist1LookPoint);

        yield return new WaitForSeconds(1);

        //Nurse
        aux = Case3Resources.Instance.nurse.GetComponent<SimpleWaypointMovement>();
        aux.SetPathAndPlay(Case3Resources.Instance.waypointsExit, Case3Resources.Instance.endoscopist1LookPoint);

        yield return new WaitForSeconds(1);

        //anestesiologa
        aux = Case3Resources.Instance.anaesthesistNurse.GetComponent<SimpleWaypointMovement>();
        aux.SetPathAndPlay(Case3Resources.Instance.waypointsExit, Case3Resources.Instance.endoscopist1LookPoint);

        yield return new WaitForSeconds(1);
        //Extra nurse
        if (isExtraNurse)
        {
            aux = Case3Resources.Instance.extraNurse.GetComponent<SimpleWaypointMovement>();
            aux.SetPathAndPlay(Case3Resources.Instance.waypointsExit, Case3Resources.Instance.endoscopist1LookPoint);
            isExtraNurse = false;
        }

        yield return new WaitForSeconds(1);

        //Endoescopista2
        aux = Case3Resources.Instance.endoscopist2.GetComponent<SimpleWaypointMovement>();
        aux.SetPathAndPlay(Case3Resources.Instance.waypointsExit, Case3Resources.Instance.endoscopist1LookPoint);

    }

    #endregion

    public void Destroy()
    {
        Destroy(this.gameObject);
    }

}
