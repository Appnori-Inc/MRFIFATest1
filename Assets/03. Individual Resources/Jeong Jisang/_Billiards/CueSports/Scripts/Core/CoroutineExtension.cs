using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public static class CoroutineExtension
{
    /// <param name="onComplete">boolean : complete by condition?</param>
    public static IEnumerator WaitforTimeWhileCondition(this MonoBehaviour runner, float time, Func<bool> condition, Action<bool> onComplete = null)
    {
        bool passed = false;
        bool timeOver = false;
        var timeRoutine = runner.StartCoroutine(WaitForTime(time, () => { passed = true; timeOver = true; }));
        var conditionRoutine = runner.StartCoroutine(WaitWhileConditino(condition, () => passed = true));
        yield return new WaitUntil(() => passed);

        onComplete?.Invoke(!timeOver);

        yield break;
    }

    private static IEnumerator WaitForTime(float time, Action onComplete)
    {
        yield return YieldInstructionCache.WaitForSeconds(time);
        onComplete?.Invoke();
    }
    private static IEnumerator WaitWhileConditino(Func<bool> condition, Action onComplete)
    {
        yield return new WaitWhile(condition);
        onComplete?.Invoke();
    }

    /// <summary>
    /// every update frame, call while runtime
    /// </summary>
    /// <param name="onUpdate">float value is normalized time (0->1)</param>
    /// <returns></returns>
    public static IEnumerator Easy(float runtime, Action<float> onUpdate)
    {
        float t = 0;
        while (t < runtime)
        {
            onUpdate?.Invoke(t / runtime);
            t += Time.deltaTime;
            yield return null;
        }

        onUpdate?.Invoke(1);
    }
}

public class CoroutineWrapper
{
    public static CoroutineWrapper Generate(MonoBehaviour runner) => new CoroutineWrapper(runner);

    public CoroutineWrapper(MonoBehaviour runner) => Runner = runner;



    public Action OnCompleteOnce { get; set; }

    //necessary Data
    public MonoBehaviour Runner { get; private set; }
    public Coroutine Routine { get; private set; }

    public bool IsPlaying { get => Routine != null; }

    private IEnumerator Target;

    public CoroutineWrapper Start(IEnumerator target)
    {
        Target = target;
        Runner.StartCoroutine(RunTarget());

        return this;
    }

    /// <summary>
    /// if routine already running, routine will stop and restart
    /// </summary>
    public CoroutineWrapper StartSingleton(IEnumerator target)
    {
        if (Routine != null)
            Stop();

        return Start(target);
    }

    /// <summary>
    /// When Routine end, call once
    /// </summary>
    public CoroutineWrapper SetOnComplete(Action onComplete)
    {
        OnCompleteOnce += onComplete;

        return this;
    }

    private IEnumerator RunTarget()
    {
        this.Routine = Runner.StartCoroutine(Target);

        yield return Routine;

        Routine = null;

        OnCompleteOnce?.Invoke();
        OnCompleteOnce = null;
    }

    public void Stop()
    {
        if (Routine != null)
        {
            Runner.StopCoroutine(Routine);
            Routine = null;
        }
    }
}