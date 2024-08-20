# origami

This branch aims to attach a configuration file to the Unity project, 
so the experiment can be conducted with customized order of origami model every time without needing to rebuild or redeploy the entire project. 

Prepare the configuration document named "experiment_configuration.json". The format of configuration is as follows. There is a template in /StreamingAssets/experiment_configuration_
```
{
  "id": "1",
  "tutorial_mode": "0",
  "tutorial_sequence": "10",
  "experiment_sequence": "ABCDEFGH"
}
```
<b><u>id</u></b><br> The participant id <br><br>
<b><u>Tutorial Mode</u></b><br>
This is a 0/1 value. 0 means experiment mode and 1 means tutorial mode. It will trigger either tutorial_sequence or experiment_sequence accordingly.<br><br>
<b><u>Tutorial Sequence</u></b><br>
A sequence of two numbers (0/1) that indicates the order of tutorial.
- 0 refers to tutorial without DL
- 1 refers to tutorial with DL
<br><br>

<b><u>Experiment Sequence</u></b><br>
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

## Experiment Logging
Experiment data is logged into two files: validation_log.txt (for all validation results) and time_log.txt (for all durations).
These files are stored into a folder (date and time of when the experiment is conducted) in the directory U:\Users\<User>\AppData\Local\Packages\<Project Name>\LocalState. 

The format of the logging files, please refer to the experiment_log.txt and time_log.txt in this repository as an example. 

### Format of Log Files
time_log.txt file is in the format of:
- <b>For each step</b>: timestamp, participant_id, is_validation_activated (0 for no DL, 1 for DL), model_id (1,2,3,4,5), step (step of folding), duration_of_step
- <b>For the end of a model</b>: timestamp, participant_id, is_validation_activated (0 for no DL, 1 for DL), model_id (1,2,3,4,5), -1 (end of model, thus no step. Use -1), duration_of_model


## Progress
2024.08.19 Updated the json format and completed the tutorial section. By editing the json file to switch in between experiment and tutorial mode. <br>
2024.08.16 Added experiment logging functionality. The result of validation is logged in one file, and time related information logged into another file. <br>
2024.08.16 Fixed bugs from 08.15 <br>
2024.08.15 Found a very weird bug. When the configuration file has a sequence of length eight, Model F will SKIP OVER the origami model. <br>
As soon as the SKIP button of model F animation is clicked, the debug console says "Model F finished, moving on to next model"
But model B was fine. 
