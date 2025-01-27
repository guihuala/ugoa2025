using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnterSpecialItem
{
    void Apply();
}

public interface IStayInSpecialItem
{
    void Stay(float stayDuration);
}

public interface IExitSpecialItem
{
    void UnApply();
}
