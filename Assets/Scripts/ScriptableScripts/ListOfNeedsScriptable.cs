using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
[CreateAssetMenu(fileName = "ListOfNeeds ", menuName = "ScriptableObjects/Needs/ListOfNeeds", order = 2)]
public class ListOfNeedsScriptable : ScriptableObject
{
    public NeedProbability[] NeedProbabilities;

    public NeedScriptableObject GetNeed()
    {
        if (NeedProbabilities.Length == 0) return null;
        float totalValue = 0;
        foreach (NeedProbability _probabilities in NeedProbabilities)
            totalValue += _probabilities.probability;
        float _random = Random.Range(0, totalValue);
        float _currentValue = 0;
        foreach (NeedProbability _probabilities in NeedProbabilities)
        {
            if (_currentValue <= _random && _currentValue + _probabilities.probability > _random)
                return _probabilities.need;
            else
                _currentValue += _probabilities.probability;
        }
        return null;
    }
    [System.Serializable]
    public class NeedProbability
    {
        [SerializeField]
        public NeedScriptableObject need;
        [SerializeField]
        [Range(0.0f, 1.0f)]
        public float probability;
    }
}
