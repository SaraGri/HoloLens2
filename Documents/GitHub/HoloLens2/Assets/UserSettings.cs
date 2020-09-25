using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.SpatialAwareness;
using UnityEngine.UI;
using System.IO;
using Microsoft.MixedReality.Toolkit.WindowsMixedReality.SpatialAwareness;
using Microsoft.MixedReality.Toolkit.Experimental.UI;

public class UserSettings : MonoBehaviour
{
    //public InputField test;
    private IMixedRealitySpatialAwarenessSystem spatialAwarenessService;
    private IMixedRealitySpatialAwarenessMeshObserver observer;
    //Für UserInput
    private int index;
    private TouchScreenKeyboard keyboard;
    private string[] keyboardEntry = new string[10];
    public Text textForUserInformation;
    public Dropdown oType, materialO, detailO;
    public Toggle recalculate;
    public InputField[] userValueField = new InputField[5];
    public Button saveS, closeS;

    // Start is called before the first frame update
    void Start()
    {
        //für die Position des Zwischenspeicher-Arrays
        index = 0;
        spatialAwarenessService = CoreServices.SpatialAwarenessSystem;
        //observ = gameObject.GetComponent<GameObject>();
       // test = gameObject.GetComponent<InputField>();
        // observer = spatialAwarenessService.GetComponent<IMixedRealitySpatialAwarenessMeshObserver>();
        observer = CoreServices.GetSpatialAwarenessSystemDataProvider<IMixedRealitySpatialAwarenessMeshObserver>();
        //Usersettings-Elemente
        oType = oType.gameObject.GetComponent<Dropdown>();
        materialO = materialO.gameObject.GetComponent<Dropdown>();
        detailO = detailO.gameObject.GetComponent<Dropdown>();
        recalculate = recalculate.gameObject.GetComponent<Toggle>();
        saveS = saveS.gameObject.GetComponent<Button>();
        closeS = closeS.gameObject.GetComponent<Button>();
        //Für Inputfield-Array
        for (int i = 0; i < userValueField.Length; i++)
        {
            userValueField[i] = userValueField[i].gameObject.GetComponent<InputField>();
        }
        //Initialisieren des Tastatureingabezwischenspeichers mit Null
        for (int i = 0; i < keyboardEntry.Length; i++)
        {
            keyboardEntry[i] = null;
        }
        setLevelOfDetail(SpatialAwarenessMeshLevelOfDetail.Fine);
        keyboardEntry[7] = "SpatialAwarenessMeshLevelOfDetail: Fine";
    }

    // Update is called once per frame
    void Update()
    {

        if (keyboard != null)
        {
            keyboardEntry[index] = keyboard.text;
        }
        if (keyboard == null || keyboard.text == "")
        {
            textForUserInformation.text = "Warning: no Input";
        }
    }

    //das aktivierende Element setzt seinen Index
    public void getInputFieldIndex(int i)
    {
        index = i;
        Debug.Log(index);

    }

    public void OpenKeyboard()
    {
        keyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default, false, false, false, false);
    }


    private void userInformation(string inputFieldPlace)
    {
        textForUserInformation.text = inputFieldPlace + "\n";
    }

    public void OpenOrCloseSettingElements(bool b)
    {
        for (int i = 0; i < userValueField.Length; i++)
        {
            userValueField[i].gameObject.SetActive(b);
        }
        materialO.gameObject.SetActive(b);
        oType.gameObject.SetActive(b);
        detailO.gameObject.SetActive(b);
        saveS.gameObject.SetActive(b);
        closeS.gameObject.SetActive(b);
        textForUserInformation.gameObject.SetActive(b);
    }

    private void getObserverType()
    {
        //https://answers.unity.com/questions/1167834/how-do-you-access-the-text-value-of-the-dropdown-u.html
        switch (oType.options[oType.value].text)
        {
            case "Sphere":
                setObserverShape(Microsoft.MixedReality.Toolkit.Utilities.VolumeType.Sphere);
                keyboardEntry[5] = "Sphere";
                break;
            case "AxisAlignedCube":
                setObserverShape(Microsoft.MixedReality.Toolkit.Utilities.VolumeType.AxisAlignedCube);
                keyboardEntry[5] = "AxisAlignedCube";
                break;
            case "UserAlignedCube":
                setObserverShape(Microsoft.MixedReality.Toolkit.Utilities.VolumeType.UserAlignedCube);
                keyboardEntry[5] = "UserAlignedCube";
                break;
            case "None":
                setObserverShape(Microsoft.MixedReality.Toolkit.Utilities.VolumeType.None);
                keyboardEntry[5] = "None";
                break;
            default:
                setObserverShape(Microsoft.MixedReality.Toolkit.Utilities.VolumeType.AxisAlignedCube);
                keyboardEntry[5] = "Default: AxisAlignedCube";
                break;
        }
    }

    private void getMaterialDisplayOption()
    {

        switch (materialO.options[materialO.value].text)
        {
            case "Visible":
                setMaterialVisibleOption(SpatialAwarenessMeshDisplayOptions.Visible);
                keyboardEntry[6] = "SpatialAwarenessMeshDisplayOption: Visible";
                break;
            case "Occlusion":
                setMaterialVisibleOption(SpatialAwarenessMeshDisplayOptions.Occlusion);
                keyboardEntry[6] = "Occlusion";
                break;
            case "None":
                setMaterialVisibleOption(SpatialAwarenessMeshDisplayOptions.None);
                keyboardEntry[6] = "None";
                break;
            default:
                setMaterialVisibleOption(SpatialAwarenessMeshDisplayOptions.Visible);
                keyboardEntry[6] = "SpatialAwarenessMeshDisplayOptionDefault: Visible";
                break;
        }
    }

    private void gettLevelOfDetail() {
        switch (detailO.options[detailO.value].text)
        {
            case "Coarse":
                setLevelOfDetail(SpatialAwarenessMeshLevelOfDetail.Coarse);
                keyboardEntry[7] = "SpatialAwarenessMeshLevelOfDetail: Coarse";
                break;
            case "Custom":
                setLevelOfDetail(SpatialAwarenessMeshLevelOfDetail.Custom);
                keyboardEntry[7] = "SpatialAwarenessMeshLevelOfDetail: Custom";
                break;
            case "Fine":
                setLevelOfDetail(SpatialAwarenessMeshLevelOfDetail.Fine);
                keyboardEntry[7] = "SpatialAwarenessMeshLevelOfDetail: Fine";
                break;
            case "Medium":
                setLevelOfDetail(SpatialAwarenessMeshLevelOfDetail.Medium);
                keyboardEntry[7] = "SpatialAwarenessMeshLevelOfDetail: Medium";
                break;
            default:
                setLevelOfDetail(SpatialAwarenessMeshLevelOfDetail.Coarse);
                keyboardEntry[7] = "SpatialAwarenessMeshLevelOfDetailDefault: Coarse";
                break;
        }
    }

    private void useRecalculateNormalsToggle()
    {
        if (recalculate.isOn)
        {
            setRecalculateNormals(true);
        }

        else
        {
            setRecalculateNormals(false);
        }
    }

    private Vector3 StringToVector(int i)
    {
        //https://answers.unity.com/questions/1134997/string-to-vector3.html
        // split the items
        string[] sInput = keyboardEntry[i].Split(',');
        Vector3 result = new Vector3(
        float.Parse(sInput[0]),
        float.Parse(sInput[1]),
        float.Parse(sInput[2]));
        return result;
    }

    //save Usersettings
    public void saveUserSettings() {
        useRecalculateNormalsToggle();
        gettLevelOfDetail();
        getMaterialDisplayOption();
        getObserverType();
        if (keyboardEntry[0] != null && keyboardEntry[0] != "")
        {
            int i = int.Parse(keyboardEntry[0]);
            setTrianglesPerCubicMeter(i);
            Debug.Log(keyboardEntry[0] + i);
        }

        if (keyboardEntry[1] != null && keyboardEntry[1] != "")
        {
            int i = int.Parse(keyboardEntry[1]);
            setUpdateIntervalObserver(i);
            Debug.Log(keyboardEntry[1] + i);
        }

        if (keyboardEntry[2] != null && keyboardEntry[2] != "")
        {
            Vector3 i = StringToVector(2);
            setObservationOrigin(i);
            Debug.Log(keyboardEntry[2] + i);
        }

        if (keyboardEntry[3] != null && keyboardEntry[3] != "")
        {
            float i = float.Parse(keyboardEntry[3]);
            setPhysicsLayer(i);
            Debug.Log(keyboardEntry[3] + i);
        }

        if (keyboardEntry[4] != null && keyboardEntry[4] != "")
        {
            Vector3 i = StringToVector(4);
            setObservationExtents(i);
            Debug.Log(keyboardEntry[4] + i);
        }

        //für den test
        for (int i = 0; i < keyboardEntry.Length; i++)
        {
            Debug.Log(keyboardEntry[i] + i);
        }

    }

    //Einstellungen für den Observer

    public void setUpdateIntervalObserver(float interval) {
        //https://microsoft.github.io/MixedRealityToolkit-Unity/Documentation/SpatialAwareness/ConfiguringSpatialAwarenessMeshObserver.html
        if (0 < interval && interval < 5.1)
        {
            observer.UpdateInterval = interval;
        }
        else {
            userInformation("invalid value by UpdateIntervalObserver");
        }
    }

    public void setIsStationaryObserver(bool b)
    {
        observer.IsStationaryObserver = true;
    }

    public void setObserverShape(Microsoft.MixedReality.Toolkit.Utilities.VolumeType shape)
    {

            if (shape == Microsoft.MixedReality.Toolkit.Utilities.VolumeType.Sphere || shape == Microsoft.MixedReality.Toolkit.Utilities.VolumeType.AxisAlignedCube || shape == Microsoft.MixedReality.Toolkit.Utilities.VolumeType.None || shape == Microsoft.MixedReality.Toolkit.Utilities.VolumeType.UserAlignedCube)
            {
                observer.ObserverVolumeType = shape;
            }
            else
            {
                userInformation("invalid value by ObserverShape");
        } 
    }

    public void setObservationExtents(Vector3 extends)
    {
        if (extends.x > 0 && extends.y > 0 && extends.z > 0)
        {
            observer.ObservationExtents = extends;
        }
        else
        {
            userInformation("invalid value by ObservationExtends");
        }
    }

    public void setObservationOrigin(Vector3 origin)
    {
        /*if (origin.x > 0 && origin.y > 0 && origin.z > 0)
        {
            observer.ObservationExtents = origin;
        }
        else
        {
            Debug.Log("");
        }*/
        observer.ObservationExtents = origin;
    }

    public void setPhysicsLayer(float layer)
    {
        if (0 <= layer && layer <= 5 || layer == 31)
        {
            observer.UpdateInterval = layer;
        }
        else
        {
            userInformation("invalid value by PhysicLayer");
        }
    }

    public void setRecalculateNormals(bool recttrue) {
        observer.RecalculateNormals = recttrue;
    }

    //Specifying None causes the observer to not render the mesh
    public void setMaterialVisibleOption(SpatialAwarenessMeshDisplayOptions option)
    {
        if (option == SpatialAwarenessMeshDisplayOptions.None || option == SpatialAwarenessMeshDisplayOptions.Occlusion || option == SpatialAwarenessMeshDisplayOptions.Visible)
        {
            observer.DisplayOption = option;
        }
        else
        {
            userInformation("invalid value by SpatialAwernessMeshDisplayOption");
        }
    }

    public void setLevelOfDetail(SpatialAwarenessMeshLevelOfDetail detail)
    {

        if (detail == SpatialAwarenessMeshLevelOfDetail.Coarse || detail == SpatialAwarenessMeshLevelOfDetail.Custom || detail == SpatialAwarenessMeshLevelOfDetail.Fine || detail == SpatialAwarenessMeshLevelOfDetail.Medium)
        {
            observer.LevelOfDetail = detail;
            Debug.Log(observer.LevelOfDetail.ToString());
        }
        else
        {
            userInformation("invalid value by SpatialAwarenessMeshLevelOfDetail");
        }
    }

    //When using the custom Level of Detail, specifies the requested value for the triangle density for the spatial mesh.
    public void setTrianglesPerCubicMeter(int interval)
    {
        if (-1 < interval && interval < 10)
        {
            observer.TrianglesPerCubicMeter = interval;
        }
        else
        {
            userInformation("invalid value by TrianglesPerCubicMeter");
        }
        //Debug.Log(observer.TrianglesPerCubicMeter.ToString());
    }

    //material durch User setzen lassen
}
