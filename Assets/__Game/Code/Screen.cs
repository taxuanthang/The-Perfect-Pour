using UnityEngine;

public abstract class Screen : MonoBehaviour
{
    protected ScreenManager _screenManager;
    protected object _data;

    public virtual void SetUp(ScreenManager screenManager)
    {
        this._screenManager = screenManager;
        AssignButtons();
    }
    public virtual void OnEnter(object data)
    {
        this._data = data;
        if (_screenManager == null)
        {
            Debug.Log($"Chua gan screenManager cho{this.name}");
        }
    }
    public abstract void OnExit();

    public abstract void AssignButtons();

    public virtual void UpdateUI<T>(T Data) { }
}
