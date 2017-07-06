using UnityEngine;
using System.Collections;


public class ScrollPathLine : ScrollPath {

	public Color lineColor = new Color(1,0.5f,0);


	public Vector3 extraBounds = Vector3.zero;
	// Despite the linear path, character can move a bit around it,
	// each point in the line are bounded with this extra bounds


	private Vector3 previousPointInLine = new Vector3(float.MinValue, float.MinValue, float.MinValue);
	// Store previous point in line for better heuristics


	public override void OnDrawGizmos() {
		// Draw lines connecting all consecutive pin points

		if (pinPoints.Count == 0) return;
		Gizmos.color = lineColor;
		Gizmos.DrawLine(transform.position, transform.position + pinPoints[0]);
		for (int i = 0 ; i < pinPoints.Count-1 ; ++i){
			Gizmos.DrawLine(transform.position + pinPoints[i],transform.position + pinPoints[i+1]);
		}

		base.OnDrawGizmos();

	}

	
	private Vector3 RemoveUnwantedDimensions(Vector3 vec) {
		// Return a copy of the given vector with only
		// the needed dimensions to compute distances

		return new Vector3(scrollOptions.catchX ? vec.x : 0,
		                   scrollOptions.catchY ? vec.y : 0,
		                   scrollOptions.catchZ ? vec.z : 0
		                   );
	}


	private float FactorizedDistance(Vector3 pt1, Vector3 pt2){
		// Distance between 2 points considering only
		// the required dimensions (x, y and / or z)

		float dx, dy, dz;
		dx = dy = dz = 0;
		if (scrollOptions.catchX) {
			dx = pt2.x-pt1.x;
			dx *= dx;
		}
		if (scrollOptions.catchY) {
			dy = pt2.y-pt1.y;
			dy *= dy;
		}
		if (scrollOptions.catchZ) {
			dz = pt2.z-pt1.z;
			dz *= dz;
		}
		return Mathf.Sqrt(dx+dy+dz);
	}

	private float FactorFromClosestPointInLine(Vector3 originalPt1, Vector3 originalPt2, Vector3 originalPoint) {
		// Find the T factor of the closest point in the line
		// relative to the given point

		Vector3 pt1ToPointVector;
		Vector3 lineVector;
		float lineMagnitude;
		float dotProduct;

		Vector3 pt1 = RemoveUnwantedDimensions(originalPt1);
		Vector3 pt2 = RemoveUnwantedDimensions(originalPt2);
		Vector3 point = RemoveUnwantedDimensions(originalPoint);

		pt1ToPointVector = point - pt1;
		lineVector = pt2 - pt1;
		
		lineMagnitude = lineVector.sqrMagnitude;
		
		dotProduct = Vector3.Dot(pt1ToPointVector, lineVector);
		
		return dotProduct / lineMagnitude;
	}


	private Vector3 GetClosestPointWithExtraBounds(Vector3 point, Vector3 pointInLine) {
		// Get the closest point between point and pointInLine,
		// considering extra bounds around pointInLine

		Vector3 closestPoint = new Vector3(pointInLine.x, pointInLine.y, pointInLine.z);
		if (extraBounds != Vector3.zero) {
			// Clamp distance with extra boundaries
			Vector3 distance = point - closestPoint;
			distance = new Vector3(Mathf.Clamp(distance.x,-extraBounds.x, extraBounds.x),
			                       Mathf.Clamp(distance.y,-extraBounds.y, extraBounds.y),
			                       Mathf.Clamp(distance.z,-extraBounds.z, extraBounds.z)
			                       );
			
			closestPoint += distance;
		}
		return closestPoint;
	}


	public Vector3 FindClosestPointInPath(Vector3 point) {
		// Loop through all path line segments to find the
		// closest point relative to the given point

		float shortestDistance = float.MaxValue;
		Vector3 closestPoint = new Vector3(float.MinValue, float.MinValue, float.MinValue);
		Vector3 pt1 = transform.position;	// first line point is self position
		Vector3 pt2;						// second line point

		// vars for closest point in each line segment
		float lerpT;
		Vector3 pointInSegment;
		float distance;
		
		for (int i = 0 ; i < pinPoints.Count ; ++i){
			// get closest point for each segment,
			// store the closest
			pt2				= transform.position + pinPoints[i];
			lerpT			= FactorFromClosestPointInLine(pt1, pt2, point);
			pointInSegment	= Vector3.Lerp(pt1, pt2, lerpT);
			distance		= FactorizedDistance(point, pointInSegment);
			
			if (distance < shortestDistance) {
				shortestDistance = distance;
				closestPoint	 = pointInSegment;
			}
			
			pt1 = pt2;
		}

		return closestPoint;
	}


	public override Vector3 EnforceLimits(Vector3 point) {

		// Find closest point in path lines
		Vector3 closestPoint = FindClosestPointInPath(point);
		if (closestPoint.x == float.MinValue)
			closestPoint = previousPointInLine;
		previousPointInLine = closestPoint;

		// check closest point within extra bounds
		return base.EnforceLimits( GetClosestPointWithExtraBounds(point, closestPoint) );
	}


	void Update() {
		// Debug lines in editor
		Debug.DrawLine(previousPoint, previousPointInLine);
		Debug.DrawLine(previousPoint, previousPoint + new Vector3(0,50,0), Color.blue);
	}

}
