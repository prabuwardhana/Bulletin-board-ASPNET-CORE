using System.Collections.Generic;
using System.Threading.Tasks;
using BulletinBoard.Entities.Models;

namespace BulletinBoard.Contracts
{
    public interface ITopicRepository
    {
        Task<IEnumerable<Topic>> GetTopicsForForumWithReplyAsync(int forumId, bool trackChanges);
        Task<Topic> GetTopicByIdAsync(int topicId, bool trackChanges);
        Task<Topic> GetTopicDetailByIdAsync(int topicId, bool trackChanges);
        Task<IEnumerable<Topic>> GetInverseReplyToTopicAsync(int topicId, bool trackChanges);
        Task<IEnumerable<Topic>> GetDescendantTopicsAsync(int topicId, bool trackChanges);
        Task<IEnumerable<Topic>> GetTopTopicsAsync(bool trackChanges);
        void CreateTopicForForum(Topic topic);
        void DeleteTopic(Topic topic);
        void UpdateTopic(Topic topic);
        void CascadeDeleteTopic(IEnumerable<Topic> topic);
    }    
}