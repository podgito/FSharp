namespace TypeProviders

open System
open FSharp.Data

//Go to Google Takeout to download Hangouts chat history!

    
type private HangoutsJson = FSharp.Data.JsonProvider<"Hangouts.json">

type MessageRecord = {
    Message:string
    Sender:string
    Timestamp:System.DateTime
    MessageType:string
}


type HangoutsFileParser() =
    member this.Parse(stream : IO.Stream) =
        let root = HangoutsJson.Load(stream)
        
        seq { for conversationCore in root.ConversationState do
                    let participants = conversationCore.ConversationState.Conversation.ParticipantData
                    for event in conversationCore.ConversationState.Event do

                            let sender = 
                                participants 
                                |> Array.filter (fun p -> p.Id.GaiaId = event.SenderId.GaiaId) 
                                |> Array.exactlyOne

                            let message = 
                                event.ChatMessage.MessageContent.Segment 
                                |> Array.exactlyOne

                            yield { 
                                Message = message.Text; 
                                Sender = sender.FallbackName; 
                                Timestamp = DateTime.Now; 
                                MessageType = message.Type 
                                }

            }