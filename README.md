# origami

This branch aims to attach a configuration file to the Unity project, 
so the experiment can be conducted with customized order of origami model every time without needing to rebuild or redeploy the entire project. 

Prepare the configuration document named "experiment_configuration.json". The format of configuration is as follows:
```
{
  "experiment_sequence": "ABCDGHEF"
}
```
Here, each alphabet from A to H represents a specific model (Origami model 2-5) and a setting of deep-learning model (activated or non-activated).
What each alphabet represents is as shown below;
- A: Model 2 (hat) with DL
- B: Model 3 (box) with DL
- C: Model 4 (fly) with DL
- D: Model 5 (yacht) with DL
- E: Model 2 (hat) without DL
- F: Model 3 (box) without DL
- G: Model 4 (fly) without DL
- H: Model 5 (yacht) without DL

Before the experiment, prepare the document. Then, go to the Device Web Portal of HoloLens (enter the IP address into browser). Upload experiment_configuration.json file 
to the directary U:\Users\<User>\AppData\Local\Packages\<Project Name>\LocalCache. 