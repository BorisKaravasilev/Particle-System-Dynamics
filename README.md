# Particle System Dynamics

Simulation of particle dynamics implemented in Unity based on the paper ["Advanced Character Physics"](https://www.cs.cmu.edu/afs/cs/academic/class/15462-s13/www/lec_slides/Jakobsen.pdf) by Thomas Jakobsen and the course notes from [SIGGRAPH-97 by Andrew Witkin](https://www.cs.cmu.edu/~baraff/sigcourse/notesc.pdf).

The simulation is implemented both with an object-oriented approach and a data-oriented approach by using DOTS to compare performance.

***Note:** Turn on gizmos in the game window to see the constraints.*

## Chain Demo

![chain](chain.gif)

## Cloth Demo

![cloth](cloth.gif)

## Cloth Demo - OOP
The constraints in the first two rows are stretched out for unknown reason. This issue is being investigated. It is probably caused by a bug in the cloth spawner.

![cloth oop](cloth-oop.gif)
