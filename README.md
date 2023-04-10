# Kage No Shuuen

https://vimeo.com/814520524 (video trailer)

![image 3](https://user-images.githubusercontent.com/71871620/230915125-a4170bee-cb97-4571-a2f3-2683df3572af.png)


Kage no Shuuen (Shadow's Demise) is a 3D stealth-action game reminiscent of the old Tenchus from the PSX days.
The project was done with scalability and maintainable architecture in mind. 
It features:
- Player behaviour through an abstracted FSM pattern.
- Enemy behaviour through a flexible Behaviour Tree pattern.
- Weapon System with a flexible architecture based on inheritance. Weapons can be nearly any type or all types at the same time (E.g. a sword can be thrown, you can physically attack with shurikens equipped).
- Animations making full use of the animator in terms of layers, masks, Behaviours and events. Through Animator Controller Overrides a great variety of animations have been achieved with a single animator, shared by all entities.
- Enemy waves through Object Pools pattern and Scriptable Objects to easily create enemy waves.

This project also made use of other Unity features like IK rigging and HDRP lighting (sky and volumetric fog).

To play the game you can download the Windows build on the game's itch.io page: https://outergazer.itch.io/kage-no-shuuen-shadows-demise
