export interface MessageSummary {
    id: number;
    from: string;
    messageId: string;
    to: string;
    timeStamp: Date;
    subject: string;
}

export interface MessageSummaries {
    messages: MessageSummary[];
}

export interface SessionChatter {
    direction: number;
    data: string;
    timeStamp: Date;
}

export interface SessionSummaries {
    sessions: SessionSummary[];
}

export interface SessionSummary {
    date: string;
    id: number;
    sessionId: string;
    sessionStartTime: Date;
    sessionEndTime: Date;
    remoteIp: string;
    username: string;
}

export interface SessionDetail {
    id: number;
    messageSummaries: MessageSummaries;
    sessionChatter: SessionChatter[];
    sessionId: string;
    sessionStartTime: Date;
    sessionEndTime: Date;
    remoteIp: string;
    username: string;
}