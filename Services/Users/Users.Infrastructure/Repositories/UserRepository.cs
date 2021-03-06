﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Users.Domain.AggregateModel.UserAggregate;
using Users.Domain.SeedWork;
using Dapper;
using Profile = Users.Domain.AggregateModel.UserAggregate.Profile;

namespace Users.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IDbConnectionFactory _dbConnectionFactory;
        private readonly IFriendshipRequestRepository _friendshipRequestRepository;
        private readonly IFriendshipRepository _friendshipRepository;

        public IUnitOfWork UnitOfWork { get; }

        public UserRepository(
            IUnitOfWork unitOfWork, 
            IDbConnectionFactory dbConnectionFactory,
            IFriendshipRequestRepository friendshipRequestRepository,
            IFriendshipRepository friendshipRepository)
        {
            _dbConnectionFactory = dbConnectionFactory;
            _friendshipRequestRepository = friendshipRequestRepository;
            UnitOfWork = unitOfWork;
            _friendshipRepository = friendshipRepository;
        }

        public void Create(User friendship)
        {
            var sql = $@"INSERT INTO Users (Id, UserName, Email)
                        VALUES (@{nameof(friendship.Id)}, @{nameof(friendship.UserName)}, @{nameof(friendship.Email)});";
            UnitOfWork.AddOperation(friendship, async connection =>
            {
                await connection.ExecuteAsync(sql, new
                {
                    friendship.Id,
                    friendship.UserName,
                    friendship.Email
                });
            });

            if (friendship.Profile != null)
            {
                AddProfile(friendship.Id, friendship.Profile);
            }
        }

        public void Update(User user)
        {
            var sql1 = $@"UPDATE Users SET
                            UserName = @{nameof(user.UserName)},
                            Email = @{nameof(user.Email)}
                        WHERE Users.Id = @{nameof(user.Id)}";
            UnitOfWork.AddOperation(user, async connection =>
            {
                await connection.ExecuteAsync(sql1, new
                {
                    user.UserName,
                    user.Email,
                    user.Id
                });
            });

            if (user.HasFlag(User.Flags.ProfileAdded))
            {
                AddProfile(user.Id, user.Profile);
                user.ClearFlag(User.Flags.ProfileAdded);
            }
            else if (user.HasProfile)
            {
                UpdateProfile(user.Profile);
            }
        }

        public async Task<User> GetByIdAsync(string userId)
        {
            var friendshipRequests = await _friendshipRequestRepository.GetByUserIdAsync(userId);
            var friendships = await _friendshipRepository.GetByUserIdAsync(userId);

            var sql = $@"SELECT Users.*, Profiles.* 
                        FROM Users
                        LEFT JOIN UserProfileMap ON UserProfileMap.UserId = Users.Id
                        LEFT JOIN Profiles ON UserProfileMap.ProfileId = Profiles.Id
                        WHERE Users.Id = @{nameof(userId)}
                        LIMIT 1";

            using (var connection = await _dbConnectionFactory.OpenConnectionAsync())
            {
                var result = await connection.QueryAsync<dynamic, Profile, (dynamic, Profile)>(
                    sql,
                    (u, p) => (u, p),
                    new
                    {
                        userId
                    },
                    splitOn: "id");
                var (user, profile) = result.FirstOrDefault();

                return user == null ? null : new User(user.Id, user.UserName, user.Email, profile, friendshipRequests, friendships);
            }
        }

        public void DeleteById(string id)
        {
            throw new NotImplementedException();
        }

        private void AddProfile(string userId, Profile profile)
        {
            var sql = $@"INSERT INTO Profiles (Age, Sex, Location)
                        VALUES (@{nameof(Profile.Age)}, @{nameof(Profile.Sex)}, @{nameof(Profile.Location)});
                        INSERT INTO UserProfileMap (UserId, ProfileId)
                        VALUES (@{nameof(userId)}, LAST_INSERT_ID());";
            UnitOfWork.AddOperation(profile, async connection =>
                await connection.ExecuteAsync(sql, new
                {
                    profile.Age,
                    profile.Sex,
                    profile.Location,
                    userId
                }));
        }

        private void UpdateProfile(Profile profile)
        {
            var sql = $@"UPDATE Profiles SET
                            Age = @{nameof(Profile.Age)},
                            Sex = @{nameof(Profile.Sex)},
                            Location = @{nameof(Profile.Location)}
                        WHERE Profiles.Id = @{nameof(profile.Id)}";
            UnitOfWork.AddOperation(profile, async connection =>
                await connection.ExecuteAsync(sql, new
                {
                    profile.Age,
                    profile.Sex,
                    profile.Location,
                    profile.Id
                }));
        }
    }
}
