using UnityEngine;

public class FloatingProjectile : MonoBehaviour
{

	#region Variables
	
	#region Public Variables
	
	[Header("Movement Settings"), Tooltip("Move Forward Speed")]
    public float forwardSpeed = 5f;
    
    [Space]
    
    [Header("Floating Settings"), Tooltip("Float Curve - Use Sin Curve For Example")]
    public AnimationCurve floatCurve;
    
    [Tooltip("Floating Magnitude")]
    public float floatMagnitude = 3f;

    [Tooltip("Time In Which Float Curve Will Restart")]
    public float floatCurveRestartTime = 1f;
    
    #endregion

    #region Private Variables
    
    private Rigidbody _rb;
    private float _tickTime;
    
    #endregion
    
    #endregion

    #region Methods
    
    // Use this for initialization
	private void Start ()
	{
		_rb = GetComponent<Rigidbody>();
	}
	
	// FixedUpdate is called Per Fixed Time step
	private void FixedUpdate ()
	{
		_tickTime += Time.deltaTime;
		if (_tickTime > floatCurveRestartTime)
			_tickTime = 0f;
		
		Vector3 movementThisFrame = transform.forward * forwardSpeed + transform.up * (floatCurve.Evaluate(_tickTime) * floatMagnitude);
		
		_rb.MovePosition(transform.position + movementThisFrame);
	}
	
	#endregion
	
}
