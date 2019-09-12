# Clock.Net #

This project aim to provide an abstract layer to time related facilities in C#.

## Claasification ##

People use clock in 2 different ways:

1. To get a readable timepoint of now. (Simply call `DateTime.Now`)
1. To get a readable duration. (Call `DateTime.Now` twice & use `end - start` to obtain the duration)

People sometimes need to know the feature of clock:

1. High resolution or not
1. Monotonic or not

People sometimes just want a logical clock indicating the sequence of events instead of a readable timepoint.
