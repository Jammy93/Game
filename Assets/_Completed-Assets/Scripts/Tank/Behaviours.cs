using UnityEngine;

using NPBehave;
using System.Collections.Generic;


namespace Complete
{
    /*
    Example behaviour trees for the Tank AI.  This is partial definition:
    the core AI code is defined in TankAI.cs.

    Use this file to specifiy your new behaviour tree.
     */
	public partial class TankAI : MonoBehaviour
	{

	


		private Root CreateBehaviourTree() {

			switch (m_Behaviour) {

			case 0:
				return FunBehaviour();
			case 1:
				return DeadlyBehaviour();
			case 2:
				return FrightenedBehaviour();
			case 3:
				return UnpredictableBehaviour();

			default:
				return new Root(new Action(() => Turn(0.1f)));
			}
		}

		/* Actions */ /* by using these actions and altering them i can set new values for the harder difficulties */

		//stops the tanks turning
		private Node StopTurning() {
			return new Action(() => Turn(0));
		}
		// stops the tanks from moving
		private Node StopMove()
		{
			return new Action(() => Move(0));

		}






		// Tank Shooting
		private Node RandomFire() {
			return new Action(() => Fire(UnityEngine.Random.Range(0.0f, 1.0f)));
		}

		// new node for diffrent rate of tank shooting
		private Node StraightFire() {
			return new Action(() => Fire(UnityEngine.Random.Range(0.0f, 0.4f)));
		}





		// modified from trackbehaviour example
		private Root FunBehaviour() {
			return new Root(new Service(0.5f, UpdatePerception,
				new Sequence(
					new Action(() => Move((float)blackboard["move"])),
					new Selector(


						new BlackboardCondition("targetOffCentre",
							Operator.IS_SMALLER_OR_EQUAL, 0.1f,
							Stops.IMMEDIATE_RESTART,
							// Stop turning and fire

						new Sequence(StopTurning(),
							StopMove(),
							new Wait(2f),
							StraightFire())),

						new BlackboardCondition("targetDistance",
							Operator.IS_GREATER_OR_EQUAL, 18f,
							Stops.IMMEDIATE_RESTART,
							// Turn right toward target
							new Action(() => Turn(0.5f))),
						// Turn left toward target
						new Action(() => Turn(-0.5f))
					)
				)));
		}

	


		private Root DeadlyBehaviour()
		{
		return new Root(new Service(0.2f, UpdatePerception,
					new Selector(

					//new blackboard for the AI when the opponent is in front of the AI it will then start fireing
						new BlackboardCondition("targetOffCentre",
							Operator.IS_SMALLER_OR_EQUAL, 0.1f,
							Stops.IMMEDIATE_RESTART,
							// Stop turning and fire
							new Sequence(StopTurning(),
								StopMove(),
								new Wait(0.2f),
								StraightFire())),
						new BlackboardCondition("navigate",  // if the AI touches a static object on the map it will navigate a new path
							Operator.IS_EQUAL, true,
							Stops.IMMEDIATE_RESTART,

							new Sequence(
								StopMove(),
								new Action(() => Turn(0.7f)),
								new Action(() => Move(0.5f)))),
						
						new BlackboardCondition("targetInFront",  // if the opponent is in front the AI will move forward
							Operator.IS_EQUAL, true,
							Stops.IMMEDIATE_RESTART,

							new Action(() => Move(0.7f))),

						new BlackboardCondition("targetOnRight",  // if on right AI will move right
							Operator.IS_EQUAL, true,
							Stops.IMMEDIATE_RESTART,


							// Turn right toward target
							new Action(() => Turn(0.5f))),
						// Turn left toward target

						new Action(() => Turn(-0.6f))



					)
				)
			);

		}

		

	

		private Root FrightenedBehaviour()
				{


					return new Root(
						new Service(0.2f, UpdatePerception,
							new Selector(
						
								new BlackboardCondition("targetOffCentre",
									Operator.IS_SMALLER_OR_EQUAL, 0.1f,
									Stops.IMMEDIATE_RESTART,

									// Stop turning and fire
									new Sequence(StopTurning(),
										StopMove(),
										new Wait(0.2f),
										StraightFire())),
								new BlackboardCondition("navigate",
									Operator.IS_EQUAL, true,
									Stops.IMMEDIATE_RESTART,
									new Sequence(
										StopMove(),
										new Action(() => Turn(0.7f)),
										new Action(() => Move(0.5f)))),
								new BlackboardCondition("targetInFront",
									Operator.IS_EQUAL, true,
									Stops.IMMEDIATE_RESTART,
									new Action(() => Move(0.5f))),
								new BlackboardCondition("targetOnRight",
									Operator.IS_EQUAL, true,
									Stops.IMMEDIATE_RESTART,
									// Turn right toward target
									new Action(() => Turn(0.6f))),
								// Turn left toward target
								new Action(() => Turn(-0.5f))



							)
						)
					);

				}



		private Root UnpredictableBehaviour()
		{

			return new Root(
				new Service(0.2f, UpdatePerception,
					new Selector(

						new BlackboardCondition("targetOffCentre",
							Operator.IS_SMALLER_OR_EQUAL, 0.1f,
							Stops.IMMEDIATE_RESTART,
							// Stop turning and fire
							new Sequence(StopTurning(),
								StopMove(),
								new Wait(0.2f),
								StraightFire())),
						new BlackboardCondition("targetOnRight",
							Operator.IS_EQUAL, true,
							Stops.IMMEDIATE_RESTART,
							// Turn right toward target
							new Action(() => Turn(0.8f))),
						// Turn left toward target
						new Action(() => Turn(-0.5f)),

						new BlackboardCondition("targetInFront",
							Operator.IS_EQUAL, true,
							Stops.IMMEDIATE_RESTART,
							new Action(() => Move(0.5f))),

						new BlackboardCondition("targetOnRight",
							Operator.IS_EQUAL, true,
							Stops.IMMEDIATE_RESTART,
							// Turn right toward target
							new Action(() => Turn(0.8f))),
						// Turn left toward target
							new Action(() => Turn(-0.5f)),

						new BlackboardCondition("navigate",
							Operator.IS_EQUAL, true,
							Stops.IMMEDIATE_RESTART,

							new Sequence(
								StopMove(),
								new Action(() => Turn(0.5f)),
								new Action(() => Move(0.6f))))


					)
				)
			);
		}






		// using this code and altering the it will allow me to set the function of the navigate blackboard which adds an additional condition for the AI to avoid static objects
		private void UpdatePerception() {

			Vector3 targetPos = TargetTransform().position;
			Vector3 localPos = this.transform.InverseTransformPoint(targetPos);
			Vector3 heading = localPos.normalized;

			Vector3 block = transform.TransformDirection(Vector3.forward);
			bool navigate = Physics.Raycast(transform.position, block, 5);

			blackboard["targetDistance"] = localPos.magnitude;
			blackboard["targetInFront"] = heading.z > 0;
			blackboard["targetOnRight"] = heading.x > 0;
			blackboard["targetOffCentre"] = Mathf.Abs(heading.x);
			blackboard["navigate"] = navigate;


		}
	}




}

