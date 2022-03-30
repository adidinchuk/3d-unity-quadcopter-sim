using UnityEngine;

public class Quadcopter : MonoBehaviour
{
    //Quadcopter componenets
    [SerializeField] Thruster rotorFrontLeft;
    [SerializeField] Thruster rotorFrontRight;
    [SerializeField] Thruster rotorBackLeft;
    [SerializeField] Thruster rotorBackRight;

    [SerializeField] float rotorCount = 4;
    //total mass attribute to help derive required forces    

    [SerializeField] float targetThrustTMP = 4;


    private void Update()
    {
        //updateMMA(targetThrustTMP, 1);
    }
    public void updateMMA(float u1, float u2pitch, float u2roll, float u2yaw)
    {
        Vector3 baseUnitVector = Vector3.up;

        Quaternion rotation = Quaternion.Euler(GetComponent<Rigidbody>().transform.rotation.eulerAngles);
        baseUnitVector = rotation * baseUnitVector;
        
        //adjust the y thurst to account for drone rotation
        float adjustedU1 = (u1 / baseUnitVector.y) / rotorCount;                

        //set rotor speed targets
        rotorFrontLeft.setRevolutionTarget(adjustedU1 - u2pitch + u2yaw);
        rotorBackRight.setRevolutionTarget(adjustedU1 + u2pitch + u2yaw);

        rotorFrontRight.setRevolutionTarget(adjustedU1 + u2roll - u2yaw);
        rotorBackLeft.setRevolutionTarget(adjustedU1 - u2roll - u2yaw);
        
    }

    public float GetMaxThurst()
    {
        return rotorFrontLeft.getMaxRevolutionsPerMinute();
    }
    public float GetMinThurst()
    {
        return rotorFrontLeft.getMinRevolutionsPerMinute();
    }
}
