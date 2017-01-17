using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ParameterBars : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}

    public int fadeOutCount = 0;
    public float alpha = 0;
    public GameObject canvas;


    public void active() {
        fadeOutCount = 200;
    }

    public float[] values = new float[3];
	
	// Update is called once per frame

    void UpdateImageColor(GameObject obj) {
        Color color;

        if (obj.transform.GetComponent<Image>() != null) {
            color = obj.transform.GetComponent<Image>().color;
            color.a = alpha;
            obj.transform.GetComponent<Image>().color = color;
        }

        if (obj.transform.GetComponent<Text>() != null) {
            color = obj.transform.GetComponent<Text>().color;
            color.a = alpha;
            obj.transform.GetComponent<Text>().color = color;
        }
            
    }

    void updateValue(int i) {
        var obj = canvas.transform.GetChild(i).gameObject.transform.GetChild(0).gameObject;

        var size = obj.GetComponent<RectTransform>().sizeDelta;

        values[i] = Mathf.Min(Mathf.Max(values[i], 0.0f), 1.0f);


        size.x = size.x * 0.9f + values[i] * 4.9f * 0.1f;
        obj.GetComponent<RectTransform>().sizeDelta = size;

    }

	void Update () {
        fadeOutCount--;
        alpha = 0.93f * alpha + (fadeOutCount > 0 ? 0.07f:0.0f);

        canvas.transform.LookAt(Camera.main.transform);
        canvas.transform.localPosition = Vector3.zero;
        canvas.transform.position = transform.position;

        for (int i = 0; i < 3; i++) {
            UpdateImageColor(canvas.transform.GetChild(i).gameObject);
            UpdateImageColor(canvas.transform.GetChild(i).gameObject.transform.GetChild(0).gameObject);
            updateValue(i);
        }

        for (int i = 3; i < 6; i++) {
            UpdateImageColor(canvas.transform.GetChild(i).gameObject);
            UpdateImageColor(canvas.transform.GetChild(i).gameObject.transform.GetChild(0).gameObject);
        }

        /*
        var scaleinv = 1.0f / transform.parent.transform.localScale.x;

        transform.localScale = new Vector3(scaleinv, scaleinv, scaleinv);
        */

    }

}
