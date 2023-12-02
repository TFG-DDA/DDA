using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextLevel : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerMovement pm;
        if (other.TryGetComponent<PlayerMovement>(out pm))
        {
            Tracker.Instance.AddEvent(new FinNivelEvent(GameManager.instance.GetPlayedLevels(), SceneManager.GetActiveScene().name));
            GameManager.instance.SetTransitionTime(.5f);
            GameManager.instance.StartTransition(TransitionTypes.TOCARDS);
            PlayerInstance.instance.ToggleMovement(false);
        }
    }
}
