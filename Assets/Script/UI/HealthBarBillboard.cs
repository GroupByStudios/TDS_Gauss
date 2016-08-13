using UnityEngine;
using System.Collections;

public class HealthBarBillboard : MonoBehaviour
{
    Character _character;
    Transform _healthBarPivot;
    Transform _healthBar;

    float _healthPercentual;
    Vector3 _lastPosition;

    // Use this for initialization
    void Start()
    {

        _character = GetComponentInParent<Character>();
        _healthBar = this.transform.FindChild("HealthBar");
        _healthBarPivot = _healthBar.FindChild("HealthBar_Pivot");

        if (_healthBar != null)
            _healthBar.gameObject.SetActive(false);
    }



    // Update is called once per frame
    void Update()
    {
        this.transform.LookAt(new Vector3(transform.position.x, Camera.main.transform.position.y, Camera.main.transform.position.z));

        if (_character != null && _healthBarPivot != null)
        {
            _healthPercentual = _character.HitPoint.CurrentWithModifiers / _character.HitPoint.MaxWithModifiers;

            if (_healthPercentual > 0 && _healthPercentual <= 1)
            {
                _healthBar.gameObject.SetActive(true);
                _healthBarPivot.localScale = new Vector3(_character.HitPoint.CurrentWithModifiers / _character.HitPoint.MaxWithModifiers, 1, 1);
            }
            else
            {
                _healthBar.gameObject.SetActive(false);
            }
        }
    }
}
