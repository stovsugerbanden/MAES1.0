using UnityEngine;
using System.Collections;

public class middleMan : MonoBehaviour {
	bool nData = false;

	public void SetNData (bool d){
		nData = d;
	}

	public bool GetNData(){
		return nData;
	}


}
