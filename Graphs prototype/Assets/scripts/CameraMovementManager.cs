using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovementManager : MonoBehaviour
{
    Camera mainCamera;

    public float moveSpeed;

    public Vector3 currentTargetPoint;
    public bool movingTowardsTargetPoint;

    public float currentTargetFOV;
    public bool movingTowardsTargetFOV;

    void Start()
    {
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
    }

    void Update()
    {
        if (movingTowardsTargetPoint && Vector3.Distance(currentTargetPoint, mainCamera.transform.position) > 0.05f)
        {
            MoveInDirection(currentTargetPoint - mainCamera.transform.position,1f);
        } else if (movingTowardsTargetPoint) { movingTowardsTargetPoint = false; }

        if(movingTowardsTargetFOV && Mathf.Abs(currentTargetFOV - mainCamera.orthographicSize) > 0.2f)
        {
            ZoomOut((currentTargetFOV - mainCamera.orthographicSize) * 2 * Time.deltaTime);
        } else if (movingTowardsTargetFOV) {movingTowardsTargetFOV = false; }

    }



    public void MoveInDirection(Vector3 moveVector, float speed)
    {
        mainCamera.transform.position += moveVector * Time.deltaTime * speed;
    }

    public void ZoomOut(float num)
    {

        if(mainCamera.orthographicSize + num <= 5)
        {
            mainCamera.orthographicSize = 5;
            return;
        }

        if(mainCamera.orthographicSize + num >= 50)
        {
            mainCamera.orthographicSize = 50;
            return;
        }
        
        mainCamera.orthographicSize += num;

    }

    public void includeAllObject(List<Vector3> positions)
    {
        Vector3 finalPosition = new Vector3();
        Debug.Log(finalPosition);

        float furthestLeftX = positions.ToArray()[0].x;
        float furtherstRightX = positions.ToArray()[0].x;

        float furthestUpY = positions.ToArray()[0].y;
        float furthestDownY = positions.ToArray()[0].y;

        foreach(Vector3 pos in positions)
        {
            finalPosition += pos;

            if( furthestLeftX > pos.x)
            {
                furthestLeftX = pos.x;
            }if(furtherstRightX < pos.x)
            {
                furtherstRightX = pos.x;
            }

            if(furthestUpY < pos.y)
            {
                furthestUpY = pos.y;
            }if (furthestDownY > pos.y)
            {
                furthestDownY = pos.y;
            }
        }

        float totalDistanceX = furtherstRightX - furthestLeftX;
        float totalDistanceY = furthestUpY - furthestDownY;

        float totalDistance = 0;
        finalPosition /= positions.Count;
        finalPosition.z = -10;

        if (totalDistanceX / 16 > totalDistanceY /9)
        {
            totalDistance = totalDistanceX;
            finalPosition.x = (furtherstRightX + furthestLeftX) / 2;
            Debug.Log("x distance priority");
        }
        else
        {

            totalDistance = (totalDistanceY / 9) * 16;
            finalPosition.y = (furthestUpY + furthestDownY) / 2;
            Debug.Log("y distance priority");
        }



        

        movingTowardsTargetPoint = true;
        currentTargetPoint = finalPosition;

        movingTowardsTargetFOV = true;
        currentTargetFOV = totalDistance / 2.5f ;

        if(currentTargetFOV > 50) { currentTargetFOV = 50; }
        else if(currentTargetFOV < 5) { currentTargetFOV = 5; }

        Debug.Log(currentTargetFOV);

        
    }
}
