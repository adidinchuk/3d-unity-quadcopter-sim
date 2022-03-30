using UnityEngine;

public class FlightController : MonoBehaviour
{
    //desired position (only y value is considered)
    [SerializeField] Vector3 desiredPosition;

    //desired position (only y value is considered)
    Vector3 desiredAttitude = new Vector3(0,0,0);


    //drone object to control
    [SerializeField] Quadcopter quadcopter;

    //PID terms for the PID controllers, altitude and attitudes
    [SerializeField]
    [Range(0, 100)]
    float thrustPIDp, thrustPIDi, thrustPIDd;
    [SerializeField]
    [Range(0, 100)]
    float rollPitchPIDp, rollPitchPIDi, rollPitchPIDd;
    [SerializeField]
    [Range(0, 100)]
    float yawPIDp, yawPIDi, yawPIDd;

    PIDController altitudePIDController;
    PIDController pitchAttitudePIDController; //X
    PIDController rollAttitudePIDController; //z
    PIDController yawAttitudePIDController; //y

    //error values computed by the Error Estimator
    private float heightError = 0;
    private float fiPitchError = 0;
    private float fiRollError = 0;
    private float fiYawError = 0;


    // Start is called before the first frame update
    void Start()
    {
        //initialize the PID controllers - one for altitude and one for each attitude dimension (x-y-z)
        altitudePIDController = new PIDController(thrustPIDp, thrustPIDi, thrustPIDd, quadcopter.GetMaxThurst(), quadcopter.GetMinThurst());
        pitchAttitudePIDController = new PIDController(rollPitchPIDp, rollPitchPIDi, rollPitchPIDd, quadcopter.GetMaxThurst() / 4, -quadcopter.GetMaxThurst() / 4);
        rollAttitudePIDController = new PIDController(rollPitchPIDp, rollPitchPIDi, rollPitchPIDd, quadcopter.GetMaxThurst() / 4, -quadcopter.GetMaxThurst() / 4);
        yawAttitudePIDController = new PIDController(yawPIDp, yawPIDi, yawPIDd, quadcopter.GetMaxThurst() / 4, -quadcopter.GetMaxThurst() / 4);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //get the new estimated state error
        EstimateError();
        //generate u1 and u2 values using the PID controllers 
        float u1 = altitudePIDController.GetPIDOutput(heightError, Time.fixedDeltaTime);
        
        float u2pitch = pitchAttitudePIDController.GetPIDOutput(fiPitchError, Time.fixedDeltaTime);
        float u2roll = rollAttitudePIDController.GetPIDOutput(fiRollError, Time.fixedDeltaTime);
        float u2yaw = yawAttitudePIDController.GetPIDOutput(fiYawError, Time.fixedDeltaTime);        
        
        //push the updates to the Multi Motor Algorithm
        quadcopter.updateMMA(u1, u2pitch, u2roll, u2yaw);
    }

    public void EstimateError()
    {
        //calculate the delta between the current and target y position and orientation angle.   
        heightError = desiredPosition.y - quadcopter.transform.position.y;
        
        if (quadcopter.transform.rotation.eulerAngles.x > 180)        
            fiPitchError = desiredAttitude.x - (quadcopter.transform.rotation.eulerAngles.x - 360);
        else
            fiPitchError = desiredAttitude.x - quadcopter.transform.rotation.eulerAngles.x;

        if (quadcopter.transform.rotation.eulerAngles.z > 180)
            fiRollError = desiredAttitude.y - (quadcopter.transform.rotation.eulerAngles.z - 360);
        else
            fiRollError = desiredAttitude.y - quadcopter.transform.rotation.eulerAngles.z;

        if (quadcopter.transform.rotation.eulerAngles.y > 180)
            fiYawError = desiredAttitude.z - (quadcopter.transform.rotation.eulerAngles.y - 360);
        else
            fiYawError = desiredAttitude.z - quadcopter.transform.rotation.eulerAngles.y;

    }

    public void setAltitudeTarget(float altitude)
    {
        desiredPosition = new Vector3(0, altitude, 0);
    }

    public void setAttitudeTarget(Vector2 xy)
    {
        desiredAttitude = new Vector3(xy.x, xy.y, 0);
    }
     


}