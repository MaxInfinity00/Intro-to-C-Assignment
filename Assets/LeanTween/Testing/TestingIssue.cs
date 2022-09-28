using UnityEngine;
using System.Collections;
using DentedPixel;
using UnityEngine.InputSystem;

public class TestingIssue : MonoBehaviour {

	LTDescr lt,ff;
	int id,fid;

	void Start () {
		LeanTween.init();
		
		lt = LeanTween.move(gameObject,100*Vector3.one,2);
		id = lt.id;
		LeanTween.pause(id);

		ff = LeanTween.move(gameObject,Vector3.zero,2);
		fid = ff.id;
		LeanTween.pause(fid);
	}

	void Update () {
		if(Keyboard.current.aKey.wasPressedThisFrame)
		{
			// Debug.Log("id:"+id);
			LeanTween.resume(id);
		}
		if(Keyboard.current.dKey.wasPressedThisFrame)
		{
			LeanTween.resume(fid);
		}
	}
}
