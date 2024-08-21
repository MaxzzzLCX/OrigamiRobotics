# origami

This branch aims to attach a configuration file to the Unity project, 
so the experiment can be conducted with customized order of origami model every time without needing to rebuild or redeploy the entire project. 

Prepare the configuration document named "experiment_configuration.json". The format of configuration is as follows. There is a template in /StreamingAssets/experiment_configuration_
```
{
  "id": "1",
  "tutorial_mode": "0",
  "tutorial_sequence": "YN",
  "experiment_sequence": "ABCDEFGH"
}
```
<b><u>id</u></b><br> The participant id <br><br>
<b><u>Tutorial Mode</u></b><br>
This is a 0/1 value. 0 means experiment mode and 1 means tutorial mode. It will trigger either tutorial_sequence or experiment_sequence accordingly.<br><br>
<b><u>Tutorial Sequence</u></b><br>
A sequence of two letters (N/Y) that indicates the order of tutorial.
- N refers to tutorial without DL
- Y refers to tutorial with DL
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

The format of the logging files, please refer to the experiment_log_1.txt and time_log_1.txt in this repository as an example. 

### Format of Log Files
<i><b>time_log_(userID).txt</b></i> file is in the format of:
- <b>For each step</b>: timestamp, participant_id, is_validation_activated (0 for no DL, 1 for DL), model_id (1,2,3,4,5), step (step of folding), duration_of_step
- <b>For the end of a model</b>: timestamp, participant_id, is_validation_activated (0 for no DL, 1 for DL), model_id (1,2,3,4,5), -1 (end of model, thus no step. Use -1), duration_of_model
e.g. 
````
638598537662045581, 1, 0, 2, 0, 00:00:01.9080039 
638598537674700574, 1, 0, 2, 1, 00:00:01.2535003 
638598537686205566, 1, 0, 2, 2, 00:00:01.1269983 
638598537700220575, 1, 0, 2, 3, 00:00:01.3920001 
638598537709915593, 1, 0, 2, 4, 00:00:00.9605016 
638598537722470575, 1, 0, 2, 5, 00:00:01.2464992 
638598537732950571, 1, 0, 2, 6, 00:00:01.0409998 
638598537745255581, 1, 0, 2, 7, 00:00:01.2224992 
638598537755575576, 1, 0, 2, 8, 00:00:01.0145015 
638598537765975570, 1, 0, 2, 9, 00:00:01.0259984 
638598537766040579, 1, 0, 2, 10, 00:00:01.0324993 
638598537766160576, 1, 0, 2, -1, 00:00:12.3230009
````
<i><b>validation_log_(userID).txt</b></i> file is in the format of:
- timestamp, modelIndex(0,1,2,3,4), actual stage, predicted stage, the current number of failures on this model, duration of validation
e.g.
````
638598543017425599, 2, 0, 0, 1, 00:00:03.7700007 
638598543109765578, 2, 3, -1, 0, 00:00:03.8524998 
638598543245100591, 2, 3, 3, 1, 00:00:03.7525021 
638598543428595564, 2, 4, 4, 0, 00:00:03.7925025 
638598543650435583, 2, 5, -1, 0, 00:00:03.7415001 
638598543719715579, 2, 5, 5, 1, 00:00:03.7485015 
638598543903180572, 2, 6, 6, 0, 00:00:03.8105022 
638598544086190578, 2, 7, -1, 0, 00:00:03.8209987 
638598544167110582, 2, 7, -1, 1, 00:00:03.7734992 
638598544296085574, 2, 7, -1, 2, 00:00:03.7545000 
638598544369135586, 2, 7, 7, 3, 00:00:03.7860003
````


## Progress
2024.08.21 User has to pass validation OR have to fail valiation >= 3 times at the current step to move on to next step. <br>
Changed some log formatting <br>
2024.08.19 Updated the json format and completed the tutorial section. By editing the json file to switch in between experiment and tutorial mode. <br>
2024.08.16 Added experiment logging functionality. The result of validation is logged in one file, and time related information logged into another file. <br>
2024.08.16 Fixed bugs from 08.15 <br>
2024.08.15 Found a very weird bug. When the configuration file has a sequence of length eight, Model F will SKIP OVER the origami model. <br>
As soon as the SKIP button of model F animation is clicked, the debug console says "Model F finished, moving on to next model"
But model B was fine. 
