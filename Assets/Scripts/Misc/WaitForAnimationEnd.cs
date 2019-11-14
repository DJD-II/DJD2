using UnityEngine;

sealed public class WaitForAnimationEnd : CustomYieldInstruction
{
    private Animation Anim { get; }
    public override bool keepWaiting => Anim.isPlaying;

    public WaitForAnimationEnd(Animation anim)
    {
        Anim = anim;
    }
}