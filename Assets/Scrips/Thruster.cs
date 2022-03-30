using System;
using UnityEngine;

public class Thruster : MonoBehaviour
{
    //broad coefficient used for the calculation of the yaw moment
    [SerializeField] float broadTorqueCoefficient = 0.1f;

    //coenficient dermining the RPM to thrust ratio
    [SerializeField] float thrustCoeficient;

    //minimum and maximum RPM constraints
    [SerializeField] float maxRevolutionsPerMinute = 8000;
    [SerializeField] float minRevolutionsPerMinute = 0;


    //this value dictates how quickly a rotor's rotation can be changed
    [Tooltip("RPM increase/decrese per ms.")]
    [SerializeField] float spinupRate = 1000;

    //rotor state tracking
    [SerializeField] float currentRevolutionRate;
    [SerializeField] float targetRevolutionRate;

    //determines if the debug lines should be drawn/visible
    [SerializeField] bool debugLines = true;

    //rotor spin direction
    [SerializeField] bool clockwiseRotation = true;

    Rigidbody rb;

    void Start()
    {
        //find and allocate the objects rigid body
        rb = GetComponent<Rigidbody>();        
    }

    // Update is called once per physics update
    void FixedUpdate()
    {       
        AddForce();         
    }

    private void AddForce()
    {
        //update revolution rate if the rotor is not moving at the requested rate
        if (targetRevolutionRate != currentRevolutionRate)
            updateRevolutionRate();

        //calculate the current thrust and moment vectors
        Vector3 thrustVector = getThrustVector();
        Vector3 momentVector = getMomentVector();

        //this code draws the debug lines to help visualize the applied force
        if (debugLines)
        {
            //fixed body frame vector transform
            Vector3 thrustVectorFBF = thrustVector;
            Debug.DrawLine(rb.transform.position, rb.transform.position + (thrustVectorFBF * 0.2f), Color.red, 0.02f);

            Vector3 momentVectorFBF = momentVector;
            Debug.DrawLine(rb.transform.position, rb.transform.position + momentVectorFBF, Color.blue, 0.02f);
        }        

        //add the thrust and moment for this frame
        rb.AddForce(momentVector, ForceMode.Force);
        rb.AddForce(thrustVector, ForceMode.Force);
    }

    private Vector3 getThrustVector()
    {
        //derive a neutral vector where the length equals the rotor's thrust force
        Vector3 neutralThrustVector = Vector3.up * getThurstForce();
        //rotate the vector to align with the  orientation of the rotor          
        Quaternion rotation = Quaternion.Euler(rb.transform.rotation.eulerAngles);                    
        
        return rotation * neutralThrustVector;
    }

    private float getThurstForce()
    {
        return currentRevolutionRate * thrustCoeficient;
    }

    public void setRevolutionTarget(float target)
    {
        //set the target revolution, ensuring that the value lies within the min and max RPM values
        targetRevolutionRate = Mathf.Clamp(target, minRevolutionsPerMinute, maxRevolutionsPerMinute);
    }

    private void updateRevolutionRate()
    {
        float mSecondToSeconds = 1000;
        //update the rotor revolution rate clamping around the maximum and minim values
        if (currentRevolutionRate < targetRevolutionRate)
            currentRevolutionRate = Math.Min(currentRevolutionRate + Time.fixedDeltaTime * mSecondToSeconds * spinupRate, Math.Min(maxRevolutionsPerMinute, targetRevolutionRate));
        else
            currentRevolutionRate = Math.Max(currentRevolutionRate - Time.fixedDeltaTime * mSecondToSeconds * spinupRate, Math.Max(minRevolutionsPerMinute, targetRevolutionRate));
    }

    public float getMaxRevolutionsPerMinute()
    {
        return maxRevolutionsPerMinute;
    }

    public float getMinRevolutionsPerMinute()
    {
        return 0;
    }

    private Vector3 getMomentVector()
    {        
        Vector3 neutralMomentVector;       
       
        if (clockwiseRotation)        
            neutralMomentVector = (Vector3.Cross(transform.localPosition, Vector3.up)).normalized; //x        
        else        
            neutralMomentVector = (-Vector3.Cross(transform.localPosition, Vector3.up)).normalized; //-x
        
        //rotate the moment vector to
        Quaternion rotation = Quaternion.Euler(rb.transform.rotation.eulerAngles);
        neutralMomentVector = rotation * neutralMomentVector;

        neutralMomentVector *= getMomentForce(); //add the moment force magnitude
        return neutralMomentVector;
    }

    private float getMomentForce()
    {
        return (Mathf.Pow(broadTorqueCoefficient,2) * currentRevolutionRate);
    }
}
