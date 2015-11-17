using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CleanLogs : MonoBehaviour
{

	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	public void Clean(){
		Text log , pe , pr;
		log = GameObject.Find("console").GetComponent<Text>();
		pe = GameObject.Find("TextPE").GetComponent<Text>();
		pr = GameObject.Find("TextPR").GetComponent<Text>();
		pr.text = "0";
		pe.text = "0";
		log.text = "";
	}
}

