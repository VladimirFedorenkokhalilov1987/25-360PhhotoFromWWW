using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ImageLoader : MonoBehaviour 
{
	#region Serialize Field
	[SerializeField]
	private GameObject _2D;

	[SerializeField]
	private GameObject _360;

	[SerializeField]
	private Camera _cam;

	[SerializeField]
	private Renderer _renderer;

	[SerializeField]
	private MeshRenderer _texture;
	#endregion

	#region Web & button
	private string _imageUrl = "paste you`r 360 photo link here";
	private WWW _imageWWW;
	private bool _isSet;
	private bool buttonPressed = false;
	#endregion

	#region Toggles
	private bool Show360 = false;
	private bool Show2D = false;
	private bool Load = false;
	private int index = 0;
	#endregion

	#region Set image if link is correct or random if not
	void Start ()
	{
		_imageWWW = new WWW (_imageUrl);
	}

	void Update()
	{
		if(_imageWWW==null|| _isSet) return;

		if(!_imageWWW.isDone)
		{
			Debug.Log("Loading......");
			return;
		}

		if (_imageWWW.error!=null)
		{
			Debug.Log("Incorrect link? try another web resourse "+_imageWWW.error);
			_renderer.material.mainTexture = null;

			SetRandomImage ();
			buttonPressed=true;
			return;
		}

		if(_imageWWW.progress==1 && _imageWWW.error==null)
		{
			if (buttonPressed == true) 
			{
				SetImage (_imageWWW.texture);
				buttonPressed = false;
			}
		}
	}
	#endregion

	private void OnGUI()
	{
		#region Select all text in textfield
		if(buttonPressed)
			GUI.TextField (new Rect (10, 50, 500, 25), _imageWWW.url);
		
		GUI.SetNextControlName("myTextField");
		_imageUrl = GUI.TextField(new Rect (10, 10, 280, 25), _imageUrl);

		TextEditor te = (TextEditor)GUIUtility.GetStateObject(typeof(TextEditor), GUIUtility.keyboardControl);

		if (te != null)
		{
			te.SelectAll();
		}
		#endregion

		#region Hide toggles if load
		if(_imageWWW.progress==1 && _imageWWW.error==null)
		{
		Show360 = GUI.Toggle(new Rect (360, 10, 50, 25), Show360, "360");
		Show2D = GUI.Toggle(new Rect (410, 10, 50, 25), Show2D, "2D");
		Load = GUI.Toggle(new Rect (460, 10, 50, 25), Load, "load link");
		}
		#endregion

		#region 360 or 2D mode
		if (Show2D && _imageWWW.progress==1)
		{
			index++;
			if(index>0)
				Show2D = false;
			
			_cam.transform.SetParent (_2D.transform);
			_cam.transform.position = _2D.transform.position;
			_texture.material.mainTexture = _imageWWW.texture;
			Camera.main.transform.rotation = _2D.transform.rotation;
			Camera.main.GetComponent<MouseLook>().enabled=false;
		}

		if(Show360 && _imageWWW.progress==1)
		{
			index--;
			if(index<0)
				Show360 = false;
			
			_cam.transform.SetParent (_360.transform);
			Camera.main.transform.position = _360.transform.position;
			Camera.main.GetComponent<MouseLook>().enabled=true;
		}
		#endregion

		#region Load random or link image
		if (GUI.Button (new Rect (300, 10, 50, 25), "Go!") && buttonPressed==false) 
		{
			if (!Show360 || Show2D ) {
				SetRandomImage ();
				_isSet = false;
				buttonPressed = true;
			} 

			if(Load && !Show360 && !Show2D)
			{
				Set360Image ();
				_isSet = false;
				buttonPressed = true;
			}
		}
		#endregion
	}

	#region Set image methods
	void SetRandomImage ()
	{
		_imageWWW = new WWW ("https://source.unsplash.com/random");

		if(_renderer.material.mainTexture == null)
		{
			buttonPressed = true;
		}
	}

	void Set360Image ()
	{
		_imageWWW = new WWW (_imageUrl);

		if(_renderer.material.mainTexture == null)
		{
			buttonPressed = true;
		}
	}

	void SetImage (Texture2D _image)
	{
		if (_renderer == null || _renderer.material == null)
			return;

		_renderer.material.mainTexture = _image;
		_texture.material.mainTexture = _image;

		_isSet = true;
	}
	#endregion
}
