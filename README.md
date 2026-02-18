GAME DESIGN DOCUMENT: The Ghost of Kültür

Version: 1.0

Genre: First-Person Puzzle / Asymmetric Simulation

Platform: PC (Unity 6.3.2f1)

Theme: Orientation & Guidance

1. EXECUTIVE SUMMARY

The Ghost of Kültür is a 3D puzzle game where the player acts as a guardian spirit within the halls of Istanbul Kültür University. The core objective is to guide a confused freshman (NPC) through their first day of school. Since the ghost is invisible and cannot speak, the player must use environmental interaction and telekinesis to lead the student to their orientation checkpoints.

2. CORE GAMEPLAY MECHANICS

2.1. Player Abilities (The Ghost)

Omnipresence (Movement): The player can fly freely through the environment, unconstrained by gravity.

Telekinesis: The ability to pick up, move, and throw physics-based objects (chairs, books, cans) to create visual or auditory clues.

Poltergeist Interaction: Directly interacting with school infrastructure, such as opening doors, flicking light switches, or activating computer screens.

2.2. The Freshman (AI NPC)

Logic System: The NPC is programmed with a "Curiosity vs. Fear" logic.

Curiosity: Triggered by opening a door or turning on a light in the NPC's field of view.

Fear: Triggered by aggressive physics (throwing objects at the NPC) or flickering lights too rapidly.

The Fear Meter: If the Fear Meter reaches 100%, the student panics and runs away, resulting in a Game Over.

2.3. Objectives & Progression

Checkpoint 1: The Entrance: Guide the student past the security turnstiles and into the main lobby.

Checkpoint 2: The Administration: Lead the student to the Registrar's office to pick up their student ID.

Checkpoint 3: The Lecture Hall: Navigate the complex corridors to find the specific classroom for the "Orientation 101" lecture.

3. WORLD DESIGN

Environment: A 3D recreation of the Istanbul Kültür University campus.

Visual Style: The "Ghostly Realm" uses a desaturated color palette with glowing outlines (VFX), while the "Human Realm" is bright and vibrant.

Boundaries: The player is restricted to the school environment (as per project requirements).

4. TECHNICAL SPECIFICATIONS (Unity 6)

Render Pipeline: Universal Render Pipeline (URP).

AI Navigation: Unity NavMesh for NPC pathfinding.

Physics: Rigidbody-based interactions for telekinesis.

Lighting: Real-time lighting used as a primary gameplay hint system.

5. ART & AUDIO

Visual FX: Use Unity 6 VFX Graph for ghostly trails and interaction highlights.

Audio: 3D Spatial Audio is crucial. The sound of a falling chair or a door creaking must guide the NPC toward the sound source.
