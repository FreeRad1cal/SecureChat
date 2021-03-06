﻿using System.Collections.Generic;
using System.Linq;
using Users.Domain.Events;
using Users.Domain.Exceptions;
using Users.Domain.SeedWork;

namespace Users.Domain.AggregateModel.UserAggregate
{
    public class User : Entity, IAggregateRoot
    {
        public static class Flags
        {
            public const string ProfileAdded = nameof(ProfileAdded);
        }

        private string _userName;
        private string _email;
        private Profile _profile;

        public string UserName
        {
            get => _userName;
            set
            {
                _userName = value;
                AddDomainEvent(new UserNameUpdatedDomainEvent(Id, value));
            }
        }

        public string Email
        {
            get => _email;
            set
            {
                _email = value;
                AddDomainEvent(new EmailUpdatedDomainEvent(Id, value));
            }
        }

        public Profile Profile
        {
            get => _profile;
            set
            {
                if (_profile == null && value != null)
                {
                    AddFlag(Flags.ProfileAdded);
                }
                _profile = value;
            }
        }

        //public Session Session { get; private set; }

        private List<FriendshipRequest> _friendshipRequests = new List<FriendshipRequest>();

        public IEnumerable<FriendshipRequest> FriendshipRequests => _friendshipRequests;

        private List<Friendship> _friendships = new List<Friendship>();

        public IEnumerable<Friendship> Friendships => _friendships;

        // The id is not autogenerated b/c it is created in another service
        public User(string id, string userName, string email, Profile profile = null, IEnumerable<FriendshipRequest> friendshipRequests = null, IEnumerable<Friendship> friendships = null)
        {
            Id = id;
            _userName = userName;
            _email = email;
            _profile = profile;
            if (friendshipRequests != null)
            {
                _friendshipRequests.AddRange(friendshipRequests);
            }
            if (friendships != null)
            {
                _friendships.AddRange(friendships);
            }
        }

        public bool HasProfile => Profile != null;

        public void MakeFriendshipRequest(User requestee)
        {
            var pendingFriendshipRequestExists =
                _friendshipRequests.Any(req => req.RequesteeId == requestee.Id && req.IsPending);
            var friendshipExists = _friendships.Any(fr => fr.IsFor(Id, requestee.Id));

            if (pendingFriendshipRequestExists)
            {
                throw new ChatDomainException("Pending friendship request already exists");
            }

            if (friendshipExists)
            {
                throw new ChatDomainException("Friendship already exists");
            }

            var friendshipRequest = new FriendshipRequest(Id, requestee.Id);
            _friendshipRequests.Add(new FriendshipRequest(Id, requestee.Id));
        }

        public void AcceptFriendshipRequest(string requesterId)
        {
            var friendshipRequest = _friendshipRequests.FirstOrDefault(req => req.RequesterId == requesterId);
            if (friendshipRequest == null)
            {
                throw new ChatDomainException("Friendship request does not exist");
            }

            friendshipRequest.Accept();
            _friendships.Add(new Friendship(requesterId, Id));
        }

        public void RejectFriendshipRequest(string requesterId)
        {
            var friendshipRequest = _friendshipRequests.FirstOrDefault(req => req.RequesterId == requesterId);
            if (friendshipRequest == null)
            {
                throw new ChatDomainException("Friendship request does not exist");
            }

            friendshipRequest.Reject();
        }
    }
}
