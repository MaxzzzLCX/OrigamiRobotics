# AR Origami Exploration: Flexible Demonstration & Virtual Manipulation of Paper

## Flexible Demonstration

This is an extension to the previous AR instruction and validation system on origami folding. From user feedbacks collected during an experiment of 16 participants, we concluded some key factors
in enchancing user experience: automation and flexibility. Thus, an exploration on automated and flexible approach of instruction demonstration is conducted. <br>

In the `/demonstration` branch of the repository, the project aims to give user an "hands-free" experience. Via eye interactions to play animations users can focus on folding the paper. Also, 
with key milstones laid out, users can be more flexible in watching multiple steps at once instead of forced to follow step-by-step instructions. <br>

Checkout the demonstration video in /AR_Origami_Demonstration


## Virtual Manipulation of Paper

This is another extension aiming to implement a virtual, interactive, and foldable paper. Users can do folds to virtual paper just like physical paper. This tool can be used as tools in planned future
studies about "Design Space Exploration & Design Fixation" and "Haptic Feedback in Manual Assembly Tasks". 

In the `/main` branch, there is a prototype to the virtual paper. Folding can be made by hovering the hand ray on the sphere (act as a tool to select a point to be folded on the paper). Pinching
and dragging the sphere object to some other position and release. A pink fold line will show to illustrate the folding line. A panel with "next" and "back" arrow will show up. By clicking the "next"
arrow, the paper will fold a layer of paper according to the fold line defined. This procedure is repeated multiple times until the origami is finished. 

Also, to select a new point to be folded, face the palm to bring up a control menu. Toggle the "Folding Mode". When folding is disabled, you can drag the sphere around and will not trigger any
folding. After adjustment, toggle the folding mode back on and continue folding. 

Checkout the demonstration video in /AR_Origami_VirtualPaperFolding


### Future Works

Some future directions of the project includes:
- Imposing physical constraints to the virtual paper. This includes considerations about the realistic edge and paper face relationships, layer ordering, and how these affect the outcome of actual folds
- As mentioned before, possible use in future studies about "Design Space Exploration & Design Fixation" and "Haptic Feedback in Manual Assembly Tasks"