export interface MessageSummary {
    id: string;
    from: string;
    to: string;
    timeStamp: Date;
    subject: string;
}

export interface MessageSummaries {
    messages: MessageSummary[];
}

export interface SessionChatter {
    direction: string;
    data: string;
    timeStamp: Date;
}

export interface SessionSummaries {
    sessions: SessionSummary[];
}

export interface SessionSummary {
    date: string;
    id: string;
    sessionId: string;
    sessionStartTime: Date;
    sessionEndTime: Date;
    remoteIp: string;
    username: string;
}

export interface SessionDetail {
    messageSummaries: MessageSummaries;
    sessionChatter: SessionChatter[];
    sessionId: string;
    sessionStartTime: Date;
    sessionEndTime: Date;
    remoteIp: string;
    username: string;
}