import * as fromChats from './chat.reducer';
import * as fromMessages from './message.reducer';
import * as fromRoot from '../../core/reducers';
import { ActionReducerMap, createFeatureSelector, createSelector } from '@ngrx/store';

export interface ChatModuleState {
    chats: fromChats.State;
    messages: fromMessages.State
}

export interface State extends fromRoot.State {
    chatModule: ChatModuleState
}

export const reducers: ActionReducerMap<ChatModuleState> = {
    chats: fromChats.reducer,
    messages: fromMessages.reducer
};

export const getChatModuleState = createFeatureSelector<ChatModuleState>('chatModule');

// chats
export const getChatsState = createSelector(
    getChatModuleState,
    (state: ChatModuleState) => state.chats
);

export const getAllChats = createSelector(getChatsState, fromChats.selectChats);
export const getChatrooms = createSelector(getChatsState, fromChats.selectChatrooms);
export const getPrivateChats = createSelector(getChatsState, fromChats.selectPrivateChats);
export const getChatroomTotal = createSelector(getChatsState, fromChats.selectChatroomTotal);
export const getPrivateChatTotal = createSelector(getChatsState, fromChats.selectPrivateChatTotal);
export const getChatById = (id: string) => createSelector(getChatsState, fromChats.selectChatById(id));
export const getNewMessageCountById = (id: string) => createSelector(getChatsState, fromChats.selectNewMessageCountById(id));
export const getChatroomsByOwnerId = (id: string) => createSelector(getChatsState, fromChats.selectChatroomsByOwnerId(id));
export const getSelectedChatroomId = createSelector(getChatsState, fromChats.selectSelectedChatroomId);
export const getSelectedPrivateChatId = createSelector(getChatsState, fromChats.selectSelectedPrivateChatId);

// messages
export const getMessagesState = createSelector(
    getChatModuleState,
    (state: ChatModuleState) => state.messages
);

export const getMessageIds = createSelector(getMessagesState, fromMessages.selectMessageIds);
export const getAllMessages = createSelector(getMessagesState, fromMessages.selectMessages);
export const getMessageById = (id: string) => createSelector(getMessagesState, fromMessages.selectMessageById(id));
export const getMessagesByChatId = (id: string) => createSelector(getMessagesState, fromMessages.selectMessagesByChatId(id));