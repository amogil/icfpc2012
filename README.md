icfpcontest2012
===============

Team
====
* Pavel Egorov (www.skbkontur.ru)
* Alexey Mogilnikov (www.skbkontur.ru)
*	Andrew Kostousov (www.skbkontur.ru)
*	Dmitry Titarenko (www.skbkontur.ru)
*	Mikhail Khrushchev (www.skbkontur.ru)
*	Alex Fetisov (www.google.com)

Russia, Ekaterinburg city.

We had used Immutable persistent Quad-tree as a Game Field.
All operations O(log size)

Logic:
=====
Basic algorithm (GreedyBot):
---
* Safely gather closest lambda/razors, without dropping rocks.
*	If none - gather some lambda/razors with dropping rocks.
*	If none - move some rocks.
	
"Smart" algorithm (TimeAwaredBackTrackingGreedyBot):
---
*	Run basic algo. It gives us some answer.
*	Give extra priority to some lambda and run basic algo. It gives us some another answer. Choose the best one. 
*	Repeat last step while have enough time

Testing:
=======
Tests/Brains/TestGreedyBot - regression test